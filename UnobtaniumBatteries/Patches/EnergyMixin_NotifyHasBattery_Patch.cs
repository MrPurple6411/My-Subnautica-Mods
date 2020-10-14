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
        public static List<TechType> infinityBatteries = new List<TechType>() { Main.UnobtaniumBatteryPack.ItemPrefab.TechType, Main.UnobtaniumCellPack.ItemPrefab.TechType };

        [HarmonyPostfix]
        public static void Postfix(EnergyMixin __instance, InventoryItem item)
        {
            if (item != null)
            {
                TechType techType = item.item.GetTechType();

                if (infinityBatteries.Contains(techType))
                {
                    foreach (BatteryModels batteryModel in __instance.batteryModels)
                    {
                        if(batteryModel.techType == techType)
                        {
                            batteryModel.model.EnsureComponent<InfinityBehaviour>();
                            return;
                        }
                    }

                    __instance.gameObject.EnsureComponent<InfinityBehaviour>();
                    return;
                }
            }

            if (__instance.TryGetComponent(out InfinityBehaviour infinityBehaviour))
                GameObject.Destroy(infinityBehaviour);
        }
    }
}
