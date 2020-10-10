using HarmonyLib;
using InfinityBattery.MonoBehaviours;
using System.Linq;
using static EnergyMixin;

namespace InfinityBattery.Patches
{
    [HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.NotifyHasBattery))]
    public static class EnergyMixin_NotifyHasBattery_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(EnergyMixin __instance, InventoryItem item)
        {
            if(item != null)
            {
                TechType techType = item.item.GetTechType();

                if (techType == Main.InfinityPack.ItemPrefab.TechType)
                {
                    BatteryModels[] i = __instance.batteryModels.Where((x) => x.techType == techType) as BatteryModels[];

                    if ((i?.Length ?? 0) > 0)
                    {
                        i[0].model.EnsureComponent<InfinityBehaviour>();
                    }
                }
            }
        }
    }
}
