using Harmony;
using QModManager.API.ModLoading;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DropUpgradesOnDestroy
{
    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnKill))]
    public class SubRoot_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(SubRoot __instance)
        {
            Dictionary<string, InventoryItem> equipment = __instance.upgradeConsole?.modules?.equipment;
            if(equipment != null)
            {
                foreach(InventoryItem item in equipment.Values)
                {
                    GameObject gameObject = CraftData.InstantiateFromPrefab(item.item.GetTechType());
                    gameObject.transform.position = __instance.transform.position + Vector3.up;
                    gameObject.SetActive(true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnKill))]
    public class Vehicle_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(Vehicle __instance)
        {
            Dictionary<string, InventoryItem> equipment = __instance.modules?.equipment;
            if(equipment != null)
            {
                foreach(InventoryItem item in equipment.Values)
                {
                    GameObject gameObject = CraftData.InstantiateFromPrefab(item.item.GetTechType());
                    gameObject.transform.position = __instance.transform.position + Vector3.up;
                    gameObject.SetActive(true);
                }
            }
        }
    }
}