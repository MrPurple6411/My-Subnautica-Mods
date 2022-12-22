namespace AquariumOverflow
{
    using HarmonyLib;
    using System.Collections;
    using System.Reflection;
    using UnityEngine;
    using UWE;
    using BepInEx;
    using System.Linq;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "AquariumOverflow",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        #endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }

        public static bool TryOverflowIntoBioreactors(SubRoot subRoot, TechType fishType, ref int breedCount)
        {
            var bioReactors = subRoot != null ? subRoot.gameObject.GetComponentsInChildren<BaseBioReactor>() ?? new BaseBioReactor[0] : new BaseBioReactor[0];

            if(bioReactors.Length == 0)
                return breedCount > 0;

            var sizePerFish =
#if SN1
                CraftData.GetItemSize(fishType);
#else
                TechData.GetItemSize(fishType);
#endif
            var failCount = 0;
            while(failCount < bioReactors.Length && breedCount > 0)
            {
                foreach(var reactor in bioReactors)
                {
                    if(breedCount > 0 && reactor.container.HasRoomFor(sizePerFish.x, sizePerFish.y))
                    {
                        CoroutineHost.StartCoroutine(AddToReactor(subRoot, fishType, sizePerFish, reactor));
                        breedCount--;
                    }
                    else
                    {
                        failCount++;
                    }
                }
                if(failCount < bioReactors.Length)
                    failCount = 0;
            }
            return breedCount > 0;
        }

        public static IEnumerator AddToReactor(SubRoot subRoot, TechType fishType, Vector2int sizePerFish, BaseBioReactor reactor)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(fishType, false);
            yield return task;

            var prefab = task.GetResult();
            prefab.SetActive(false);

            if(!reactor.container.HasRoomFor(sizePerFish.x, sizePerFish.y))
            {
                var breedCount = 1;

                if(BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Any(x => x.Metadata.Name == "FCSEnergySolutions"))
                    AGCompat.TryOverflowIntoAlterraGens(subRoot, fishType, ref breedCount);

                if(BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Any(x => x.Metadata.Name == "CyclopsBioReactor") && breedCount > 0)
                    CBRCompat.TryOverflowIntoCyclopsBioreactors(subRoot, fishType, ref breedCount);

                if(breedCount > 0)
                    TryOverflowIntoBioreactors(subRoot, fishType, ref breedCount);

                yield break;
            }

            var gameObject = Object.Instantiate(prefab);

            var pickupable = gameObject.EnsureComponent<Pickupable>();
#if SUBNAUTICA_EXP
                TaskResult<Pickupable> taskResult = new TaskResult<Pickupable>();
                yield return pickupable.PickupAsync(taskResult, false);
                pickupable = taskResult.Get();
#else
            pickupable.Pickup(false);
#endif
            reactor.container.AddItem(pickupable);
        }
    }
}