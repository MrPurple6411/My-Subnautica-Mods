namespace AquariumOverflow
{
    using HarmonyLib;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using System.Collections;
    using System.Reflection;
    using UnityEngine;
    using UWE;

    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        public static bool TryOverflowIntoBioreactors(SubRoot subRoot, TechType fishType, ref int breedCount)
        {
            BaseBioReactor[] bioReactors = subRoot?.gameObject?.GetComponentsInChildren<BaseBioReactor>() ?? new BaseBioReactor[0];

            if(bioReactors.Length == 0)
                return breedCount > 0;

            Vector2int sizePerFish =
#if SN1
                CraftData.GetItemSize(fishType);
#else
                TechData.GetItemSize(fishType);
#endif
            int failCount = 0;
            while(failCount < bioReactors.Length && breedCount > 0)
            {
                foreach(BaseBioReactor reactor in bioReactors)
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
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(fishType, false);
            yield return task;

            GameObject prefab = task.GetResult();
            prefab.SetActive(false);

            if(!reactor.container.HasRoomFor(sizePerFish.x, sizePerFish.y))
            {
                int breedCount = 1;

                if(QModServices.Main.ModPresent("FCSEnergySolutions"))
                    AGCompat.TryOverflowIntoAlterraGens(subRoot, fishType, ref breedCount);

                if(QModServices.Main.ModPresent("CyclopsBioReactor") && breedCount > 0)
                    CBRCompat.TryOverflowIntoCyclopsBioreactors(subRoot, fishType, ref breedCount);

                if(breedCount > 0)
                    Main.TryOverflowIntoBioreactors(subRoot, fishType, ref breedCount);

                yield break;
            }

            GameObject gameObject = GameObject.Instantiate(prefab);

            Pickupable pickupable = gameObject.EnsureComponent<Pickupable>();
#if SUBNAUTICA_EXP
                TaskResult<Pickupable> taskResult = new TaskResult<Pickupable>();
                yield return pickupable.PickupAsync(taskResult, false);
                pickupable = taskResult.Get();
#else
            pickupable.Pickup(false);
#endif
            reactor.container.AddItem(pickupable);
            yield break;
        }
    }
}