#if BZ
namespace DropUpgradesOnDestroy.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [HarmonyPatch(typeof(SeaTruckSegment), nameof(SeaTruckSegment.OnKill))]
    public class SeaTruckSegment_OnKill
    {
        public static SeaTruckSegment lastDestroyed;

        [HarmonyPrefix]
        public static void Prefix(SeaTruckSegment __instance)
        {
            if(__instance.IsFront() && __instance != lastDestroyed)
            {
                lastDestroyed = __instance;
                List<InventoryItem> equipment = __instance.motor?.upgrades?.modules?.equipment?.Values?.Where((e) => e != null).ToList() ?? new List<InventoryItem>();

                Vector3 position = __instance.gameObject.transform.position;
                Main.SpawnModuleNearby(equipment, position);
            }
        }
    }
}
#endif