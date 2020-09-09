using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UWE;

namespace DropUpgradesOnDestroy.Patches
{
    [HarmonyPatch(typeof(Vehicle), "OnKill")]
    public class Vehicle_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(Vehicle __instance)
        {
            Dictionary<string, InventoryItem> eq = AccessTools.Field(typeof(Equipment), "equipment").GetValue(__instance.modules) as Dictionary<string, InventoryItem>;
            List<InventoryItem> equipment = eq?.Values?.Where((e) => e != null).ToList() ?? new List<InventoryItem>();
            Vector3 position = __instance.gameObject.transform.position;
            CoroutineHost.StartCoroutine(Main.SpawnModuleNearby(equipment, position));
        }
    }

}