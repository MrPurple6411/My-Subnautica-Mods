namespace DropUpgradesOnDestroy.Patches
{
    using HarmonyLib;
    using System.Linq;

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnKill))]
    public class Vehicle_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(Vehicle __instance)
        {
            if( __instance.modules?.equipment?.Values is null) return;

            var equipment = __instance.modules.equipment.Values.Where((e) => e != null).ToList();

            switch(__instance)
            {
                case Exosuit exosuit:
                    exosuit.storageContainer.container.ForEach((x) => equipment.Add(x));
                    exosuit.energyInterface.sources.ForEach((x) => { if(x.batterySlot.storedItem != null) equipment.Add(x.batterySlot.storedItem); });
                    break;
#if SN1
                case SeaMoth seaMoth:
                    seaMoth.energyInterface.sources.ForEach((x) => { if(x.batterySlot.storedItem != null) equipment.Add(x.batterySlot.storedItem); });
                    break;
#endif
            }

            var position = __instance.gameObject.transform.position;
            Main.SpawnModuleNearby(equipment, position);
        }
    }

}