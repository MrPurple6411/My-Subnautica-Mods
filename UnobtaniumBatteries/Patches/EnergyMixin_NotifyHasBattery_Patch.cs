namespace UnobtaniumBatteries.Patches
{
    using HarmonyLib;
    using UnityEngine;
    using MonoBehaviours;

    [HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.NotifyHasBattery))]
    public static class EnergyMixin_NotifyHasBattery_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(EnergyMixin __instance, InventoryItem item)
        {
            if(item != null)
            {
                var techType = item.item.GetTechType();

                if(Main.unobtaniumBatteries.Contains(techType))
                {
                    foreach(var batteryModel in __instance.batteryModels)
                    {
                        if (batteryModel.techType != techType) continue;
                        batteryModel.model.EnsureComponent<UnobtaniumBehaviour>();
                        return;
                    }

                    __instance.gameObject.EnsureComponent<UnobtaniumBehaviour>();
                    return;
                }
            }

            if(__instance.TryGetComponent(out UnobtaniumBehaviour infinityBehaviour))
                Object.Destroy(infinityBehaviour);
        }
    }
}