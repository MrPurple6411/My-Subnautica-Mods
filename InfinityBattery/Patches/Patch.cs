using HarmonyLib;
using InfinityBattery.MonoBehaviours;
using System.Linq;

namespace InfinityBattery.Patches
{
    [HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.NotifyHasBattery))]
    public static class Patch
    {
        [HarmonyPrefix]
        public static void Prefix(EnergyMixin __instance, InventoryItem item)
        {
            TechType techType = item.item.GetTechType();

            if (techType == Main.InfinityPack.ItemPrefab.TechType)
            {
                EnergyMixin.BatteryModels? i = __instance.batteryModels.Where((x) => x.techType == techType)?.First();

                if (i.HasValue)
                {
                    i.Value.model.EnsureComponent<InfinityBehaviour>();
                }
            }
        }
    }
}
