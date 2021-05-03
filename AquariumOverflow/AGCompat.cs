namespace AquariumOverflow
{
    using FCS_AlterraHub.Interfaces;
    using QModManager.API;
    using System.Collections;
    using UnityEngine;
    using UWE;

    public static class AGCompat
    {
        public static bool TryOverflowIntoAlterraGens(SubRoot subRoot, TechType fishType, ref int breedCount)
        {
            IFCSStorage[] AlterraGens = subRoot?.gameObject?.GetComponentsInChildren<IFCSStorage>() ?? new IFCSStorage[0];

            if(AlterraGens.Length == 0)
                return breedCount > 0;

            int failCount = 0;

            while(failCount < AlterraGens.Length && breedCount > 0)
            {
                foreach(IFCSStorage storage in AlterraGens)
                {
                    if(breedCount > 0 && storage.GetType().Name.Contains("AlterraGen") && storage.CanBeStored(1, fishType))
                    {
                        CoroutineHost.StartCoroutine(AddItemToAlterraGen(subRoot, fishType, storage));
                        breedCount--;
                    }
                    else
                    {
                        failCount++;
                    }
                }
                if(failCount < AlterraGens.Length)
                    failCount = 0;
            }

            return breedCount > 0;
        }

        private static IEnumerator AddItemToAlterraGen(SubRoot subRoot, TechType fishType, IFCSStorage container)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(fishType, false);
            yield return task;

            GameObject prefab = task.GetResult();
            prefab.SetActive(false);

            int breedCount = 1;
            if(!container.CanBeStored(breedCount, fishType))
            {
                if(QModServices.Main.ModPresent("CyclopsBioReactor"))
                    CBRCompat.TryOverflowIntoCyclopsBioreactors(subRoot, fishType, ref breedCount);

                if(breedCount > 0)
                    Main.TryOverflowIntoBioreactors(subRoot, fishType, ref breedCount);

                yield break;
            }

            if(breedCount == 0)
                yield break;

            GameObject gameObject = GameObject.Instantiate(prefab);
            Pickupable pickupable = gameObject.EnsureComponent<Pickupable>();
#if SUBNAUTICA_EXP
                TaskResult<Pickupable> taskResult = new TaskResult<Pickupable>();
                yield return pickupable.PickupAsync(taskResult, false);
                pickupable = taskResult.Get();
#else
            pickupable.Pickup(false);
#endif
            container.AddItemToContainer(new InventoryItem(pickupable));

            yield break;
        }
    }
}
