#if BZ
namespace DropUpgradesOnDestroy.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Linq;
#if SN1
    using UnityEngine;
#endif

    [HarmonyPatch(typeof(SeaTruckSegment), nameof(SeaTruckSegment.OnKill))]
    public class SeaTruckSegment_OnKill
    {
        private static SeaTruckSegment lastDestroyed;

        [HarmonyPrefix]
        public static void Prefix(SeaTruckSegment __instance)
        {
            if (!__instance.IsFront() || __instance == lastDestroyed) return;
            lastDestroyed = __instance;
            var equipment = __instance.motor?.upgrades?.modules?.equipment?.Values.Where((e) => e != null).ToList() ?? new List<InventoryItem>();

            var position = __instance.gameObject.transform.position;
            Main.SpawnModuleNearby(equipment, position);
        }
    }
}
#endif