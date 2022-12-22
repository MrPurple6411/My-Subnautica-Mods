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

    [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.ManagedUpdate))]
    internal class WaterParkCreatureUpdatePrefix
    {
        private static readonly Dictionary<WaterParkCreature, float> TimeLastGenerated = new();

        [HarmonyPrefix]
        public static void Prefix(WaterParkCreature __instance)
        {
            if (!Main.SMLConfig.EnablePowerGeneration) return;
            
            if (!__instance.gameObject.TryGetComponent(out LiveMixin liveMixin) || !liveMixin.IsAlive()) return;

            var techType = __instance.pickupable?.GetTechType() ?? TechType.None;
            if (!Main.SMLConfig.CreaturePowerGeneration.TryGetValue(techType, out var powerValue)) return;

            if(!TimeLastGenerated.TryGetValue(__instance, out var time))
            {
                time = DayNightCycle.main.timePassedAsFloat;
            }

            var power = powerValue/10 * (DayNightCycle.main.timePassedAsFloat - time) * Main.SMLConfig.PowerGenSpeed;
            var powerSource = __instance.GetWaterPark() != null ? __instance.GetWaterPark().gameObject.GetComponent<PowerSource>() : null;
            
            if(powerSource != null)
            {
                if(!powerSource.AddEnergy(power, out var amountStored) && powerSource.connectedRelay != null)
                    powerSource.connectedRelay.AddEnergy(power - amountStored, out _);
            }

            TimeLastGenerated[__instance] = DayNightCycle.main.timePassedAsFloat;
        }
    }
}
