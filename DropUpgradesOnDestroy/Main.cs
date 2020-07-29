using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace DropUpgradesOnDestroy
{
    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnKill))]
    public class SubRoot_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(SubRoot __instance)
        {
            List<InventoryItem> equipment = __instance.upgradeConsole?.modules?.equipment?.Values?.Where((e) => e != null).ToList() ?? new List<InventoryItem>();
            foreach (InventoryItem item in equipment)
            {
                GameObject gameObject = CraftData.InstantiateFromPrefab(item.item.GetTechType());
                Vector3 position = __instance.gameObject.transform.position;
                gameObject.transform.position = new Vector3(position.x + UnityEngine.Random.Range(-3, 3), position.y + UnityEngine.Random.Range(5, 8), position.z + UnityEngine.Random.Range(-3, 3));
                gameObject.SetActive(true);
            }
        }
    }

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnKill))]
    public class Vehicle_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(Vehicle __instance)
        {
            List<InventoryItem> equipment = __instance.modules?.equipment?.Values?.Where((e) => e != null).ToList() ?? new List<InventoryItem>();
            foreach (InventoryItem item in equipment)
            {
                GameObject gameObject = CraftData.InstantiateFromPrefab(item.item.GetTechType());
                Vector3 position = __instance.gameObject.transform.position;
                gameObject.transform.position = new Vector3(position.x + UnityEngine.Random.Range(-3, 3), position.y + UnityEngine.Random.Range(5, 8), position.z + UnityEngine.Random.Range(-3, 3));
                gameObject.SetActive(true);
            }
        }
    }

#if BELOWZERO

    [HarmonyPatch(typeof(SeaTruckSegment), nameof(SeaTruckSegment.OnKill))]
    public class SeaTruckSegment_OnKill
    {
        public static SeaTruckSegment lastDestroyed;

        [HarmonyPrefix]
        public static void Prefix(SeaTruckSegment __instance)
        {
            //ErrorMessage.AddMessage($"{__instance} {__instance.GetInstanceID()}");
            if(__instance.IsFront() && __instance != lastDestroyed)
            {
                lastDestroyed = __instance;
                List<InventoryItem> equipment = __instance.motor?.upgrades?.modules?.equipment.Values?.Where((e) => e != null).ToList() ?? new List<InventoryItem>();
                foreach(InventoryItem item in equipment)
                {
                    GameObject gameObject = CraftData.InstantiateFromPrefab(item.item.GetTechType());
                    Vector3 position = __instance.gameObject.transform.position;
                    gameObject.transform.position = new Vector3(position.x + UnityEngine.Random.Range(-3, 3), position.y + UnityEngine.Random.Range(5, 8), position.z + UnityEngine.Random.Range(-3, 3));
                    gameObject.SetActive(true);
                }
            }
        }
    }

#endif
}