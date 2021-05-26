namespace UnobtaniumBatteries.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using UnityEngine;
    using MonoBehaviours;
    using static Charger;

    [HarmonyPatch(typeof(Charger), nameof(Charger.OnEquip))]
    internal static class Charger_OnEquip_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(string slot, InventoryItem item, Dictionary<string, SlotDefinition> ___slots)
        {
            if (!___slots.TryGetValue(slot, out var slotDefinition)) return;
            var battery = slotDefinition.battery;
            var pickupable = item?.item;
            if(battery != null && pickupable != null && Main.unobtaniumBatteries.Contains(pickupable.GetTechType()))
            {
                battery.EnsureComponent<UnobtaniumBehaviour>();
            }
            else if(battery != null && battery.TryGetComponent(out UnobtaniumBehaviour unobtaniumBehaviour))
            {
                Object.Destroy(unobtaniumBehaviour);
            }
        }
    }
}