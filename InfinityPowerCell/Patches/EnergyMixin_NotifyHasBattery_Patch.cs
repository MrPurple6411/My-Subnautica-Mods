using HarmonyLib;
using InfinityPowerCell.MonoBehaviours;
using System.Linq;
using System.Text;
using static EnergyMixin;

namespace InfinityPowerCell.Patches
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

                if (techType == Main.InfinityCellPack.ItemPrefab.TechType)
                {

                    foreach (BatteryModels batteryModel in __instance.batteryModels)
                    {
                        if (batteryModel.techType == techType)
                        {
                            batteryModel.model.EnsureComponent<InfinityBehaviour>();
                            break;
                        }
                    }
                }
            }
        }
    }
}
