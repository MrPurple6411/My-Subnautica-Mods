namespace BetterACU
{
    using FCS_AlterraHub.Interfaces;
    using System.Collections;
    using UnityEngine;
    using UWE;

    public static class AGT
    {
        public static bool TryBreedIntoAlterraGen(WaterPark waterPark, TechType parkCreatureTechType, WaterParkCreature parkCreature)
        {
            IFCSStorage[] AlterraGens = waterPark?.gameObject?.GetComponentInParent<SubRoot>()?.gameObject?.GetComponentsInChildren<IFCSStorage>();

            if(AlterraGens is null)
                return false;

            foreach(IFCSStorage storage in AlterraGens)
            {
                if(storage.GetType().Name.Contains("AlterraGen") && storage.IsAllowedToAdd(parkCreature.pickupable, false))
                {
                    CoroutineHost.StartCoroutine(AddItemToAlterraGen(parkCreatureTechType, storage));
                    return true;
                }
            }

            return false;
        }

        private static IEnumerator AddItemToAlterraGen(TechType parkCreatureTechType, IFCSStorage container)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(parkCreatureTechType, false);
            yield return task;

            GameObject prefab = task.GetResult();
            prefab.SetActive(false);

            while(!container.CanBeStored(1, parkCreatureTechType))
                yield return new WaitForSecondsRealtime(1);

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
