#if SN1
namespace UnobtaniumBatteries.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using UnityEngine;
    using UnobtaniumBatteries.MonoBehaviours;
    using static Charger;

    [HarmonyPatch(typeof(Charger), nameof(Charger.OnEquip))]
    internal static class Charger_OnEquip_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(string slot, InventoryItem item, Dictionary<string, SlotDefinition> ___slots)
        {
            if(___slots.TryGetValue(slot, out SlotDefinition slotDefinition))
            {
                GameObject battery = slotDefinition.battery;
                Pickupable pickupable = item?.item;
                if(battery != null && pickupable != null && Main.unobtaniumBatteries.Contains(pickupable.GetTechType()))
                {
                    battery.EnsureComponent<UnobtaniumBehaviour>();
                }
                else if(battery != null && battery.TryGetComponent(out UnobtaniumBehaviour unobtaniumBehaviour))
                {
                    GameObject.Destroy(unobtaniumBehaviour);
                }
            }
        }
    }
}
#endif