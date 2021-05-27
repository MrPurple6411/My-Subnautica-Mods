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
            var componentInParent = waterPark != null ? waterPark.gameObject.GetComponentInParent<SubRoot>() : null;
            var AlterraGens = componentInParent != null
                ? componentInParent.gameObject.GetComponentsInChildren<IFCSStorage>()
                : null;

            if(AlterraGens is null)
                return false;

            foreach(var storage in AlterraGens)
            {
                if (!storage.GetType().Name.Contains("AlterraGen") ||
                    !storage.IsAllowedToAdd(parkCreature.pickupable, false)) continue;
                
                CoroutineHost.StartCoroutine(AddItemToAlterraGen(parkCreatureTechType, storage));
                return true;
            }

            return false;
        }

        private static IEnumerator AddItemToAlterraGen(TechType parkCreatureTechType, IFCSStorage container)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(parkCreatureTechType, false);
            yield return task;

            var prefab = task.GetResult();
            prefab.SetActive(false);

            while(!container.CanBeStored(1, parkCreatureTechType))
                yield return new WaitForSecondsRealtime(1);

            var gameObject = Object.Instantiate(prefab);

            var pickupable = gameObject.EnsureComponent<Pickupable>();
#if SUBNAUTICA_EXP
                TaskResult<Pickupable> taskResult = new TaskResult<Pickupable>();
                yield return pickupable.PickupAsync(taskResult, false);
                pickupable = taskResult.Get();
#else
            pickupable.Pickup(false);
#endif
            container.AddItemToContainer(new InventoryItem(pickupable));
        }
    }
}
