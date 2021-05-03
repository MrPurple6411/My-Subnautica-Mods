namespace DropUpgradesOnDestroy.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnKill))]
    public class Vehicle_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(Vehicle __instance)
        {
            List<InventoryItem> equipment = __instance?.modules?.equipment?.Values?.Where((e) => e != null).ToList() ?? new List<InventoryItem>();

            switch(__instance)
            {
                case Exosuit exosuit:
                    exosuit?.storageContainer?.container?.ForEach((x) => equipment.Add(x));
                    exosuit?.energyInterface?.sources?.ForEach((x) => { if(x.batterySlot.storedItem != null) equipment.Add(x.batterySlot.storedItem); });
                    break;
#if SN1
                case SeaMoth seaMoth:
                    seaMoth?.energyInterface?.sources?.ForEach((x) => { if(x.batterySlot.storedItem != null) equipment.Add(x.batterySlot.storedItem); });
                    break;
#endif
            }

            Vector3 position = __instance.gameObject.transform.position;
            Main.SpawnModuleNearby(equipment, position);
        }
    }

}