#if SN1
namespace UnobtaniumBatteries.Patches
{
    using HarmonyLib;
    using UnityEngine;
    using UnobtaniumBatteries.MonoBehaviours;
    using static EnergyMixin;

    [HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.NotifyHasBattery))]
    public static class EnergyMixin_NotifyHasBattery_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(EnergyMixin __instance, InventoryItem item)
        {
            if(item != null)
            {
                TechType techType = item.item.GetTechType();

                if(Main.unobtaniumBatteries.Contains(techType))
                {
                    foreach(BatteryModels batteryModel in __instance.batteryModels)
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

            if(__instance.TryGetComponent(out UnobtaniumBehaviour infinityBehaviour))
                GameObject.Destroy(infinityBehaviour);
        }
    }
}
#endif