using HarmonyLib;
using UnobtaniumBatteries.MonoBehaviours;
using System.Collections.Generic;
using UnityEngine;
using static EnergyMixin;

namespace UnobtaniumBatteries.Patches
{
    [HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.NotifyHasBattery))]
    public static class EnergyMixin_NotifyHasBattery_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(EnergyMixin __instance, InventoryItem item)
        {
            if (item != null)
            {
                TechType techType = item.item.GetTechType();

                if (Main.unobtaniumBatteries.Contains(techType))
                {
                    foreach (BatteryModels batteryModel in __instance.batteryModels)
                    {
                        if(batteryModel.techType == techType)
                        {
                            batteryModel.model.EnsureComponent<UnobtaniumBehaviour>();
                            return;
                        }
                    }

                    __instance.gameObject.EnsureComponent<UnobtaniumBehaviour>();
                    return;
                }
            }

            if (__instance.TryGetComponent(out UnobtaniumBehaviour infinityBehaviour))
                GameObject.Destroy(infinityBehaviour);
        }
    }
}
