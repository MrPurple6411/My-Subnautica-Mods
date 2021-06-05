namespace BetterACU.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using UnityEngine;

#if SUBNAUTICA_EXP
    [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.BornAsync))]
    internal class WaterParkCreature_Born_Prefix
    {
        [HarmonyPrefix]
        public static bool Prefix(WaterPark waterPark, Vector3 position)
        {
            return waterPark.IsPointInside(position);
        }
    }
#else
    [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.Born))]
    internal class WaterParkCreatureBornPrefix
    {
        [HarmonyPrefix]
        public static bool Prefix(WaterPark waterPark, Vector3 position)
        {
            return waterPark.IsPointInside(position);
        }
    }
#endif

#if SN1
    [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.Update))]
#elif BZ
    [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.ManagedUpdate))]
#endif
    internal class WaterParkCreatureUpdatePrefix
    {
        private static readonly Dictionary<WaterParkCreature, float> TimeLastGenerated = new();

        [HarmonyPrefix]
        public static void Prefix(WaterParkCreature __instance)
        {
            if (!Main.Config.EnablePowerGeneration) return;
            if ((!(__instance.GetComponent<LiveMixin>() == null || !__instance.GetComponent<LiveMixin>().IsAlive())) ||
                !Main.Config.CreaturePowerGeneration.TryGetValue(__instance.pickupable != null ? __instance.pickupable.GetTechType() : TechType.None,
                    out var powerValue)) return;
            if(!TimeLastGenerated.TryGetValue(__instance, out var time))
            {
                time = DayNightCycle.main.timePassedAsFloat;
            }

            var power = powerValue * (DayNightCycle.main.timePassedAsFloat - time) * Main.Config.PowerGenSpeed;
            var powerSource = __instance.GetWaterPark() != null ? __instance.GetWaterPark().itemsRoot.gameObject.GetComponent<PowerSource>() : null;

            if(powerSource != null)
            {
                if(!powerSource.AddEnergy(power, out var amountStored) && powerSource.connectedRelay != null)
                    powerSource.connectedRelay.AddEnergy(power - amountStored, out _);
            }

            TimeLastGenerated[__instance] = DayNightCycle.main.timePassedAsFloat;
        }
    }
}
