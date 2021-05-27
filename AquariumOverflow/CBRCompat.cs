namespace AquariumOverflow
{
    using CyclopsBioReactor;
    using QModManager.API;
    using System.Collections;
    using UnityEngine;
    using UWE;

    public static class CBRCompat
    {
        public static bool TryOverflowIntoCyclopsBioreactors(SubRoot subRoot, TechType fishType, ref int breedCount)
        {
            var cyBioReactors = subRoot.GetComponentsInChildren<CyBioReactorMono>() ?? new CyBioReactorMono[0];

            if(cyBioReactors.Length == 0)
                return breedCount > 0;

            var sizePerFish =
#if SN1
                CraftData.GetItemSize(fishType);
#else
                TechData.GetItemSize(fishType);
#endif
            var failCount = 0;
            while(failCount < cyBioReactors.Length && breedCount > 0)
            {
                foreach(var reactor in cyBioReactors)
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
                if(failCount < cyBioReactors.Length)
                    failCount = 0;
            }
            return breedCount > 0;
        }

        public static IEnumerator AddToReactor(SubRoot subRoot, TechType fishType, Vector2int sizePerFish, CyBioReactorMono reactor)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(fishType, false);
            yield return task;

            var prefab = task.GetResult();
            prefab.SetActive(false);

            if(!reactor.container.HasRoomFor(sizePerFish.x, sizePerFish.y))
            {
                var breedCount = 1;

                if(QModServices.Main.ModPresent("FCSEnergySolutions"))
                    AGCompat.TryOverflowIntoAlterraGens(subRoot, fishType, ref breedCount);

                if(breedCount > 0)
                    Main.TryOverflowIntoBioreactors(subRoot, fishType, ref breedCount);

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
