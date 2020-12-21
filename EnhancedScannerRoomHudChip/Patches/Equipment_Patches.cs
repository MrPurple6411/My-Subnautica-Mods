using HarmonyLib;
using System;
using UnityEngine;

namespace EnhancedScannerRoomHudChip.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    public static class Equipment_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Equipment.AddItem))]
        public static void AddItem_Postfix(Equipment __instance, InventoryItem newItem, bool __result)
        {
            if(__result && __instance == Inventory.main.equipment && newItem.item.GetTechType() == TechType.MapRoomHUDChip)
                Player.main.gameObject.EnsureComponent<MapRoomFunctionality_Patches.ScannerChipFunctionality>();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Equipment.RemoveItem), new Type[] { typeof(string), typeof(bool), typeof(bool) })]
        public static void RemoveItem_Postfix(Equipment __instance, InventoryItem __result)
        {
            if (__result != null && __instance == Inventory.main.equipment && __result.item.GetTechType() == TechType.MapRoomHUDChip && __instance.GetCount(TechType.MapRoomHUDChip) == 0 && Player.main.gameObject.TryGetComponent(out MapRoomFunctionality_Patches.ScannerChipFunctionality chip))
                GameObject.Destroy(chip);
        }
    }
}
