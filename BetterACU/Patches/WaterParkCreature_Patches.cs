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
    internal class WaterParkCreature_Born_Prefix
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
    internal class WaterParkCreature_Update_Prefix
    {
        public static Dictionary<WaterParkCreature, float> timeLastGenerated = new Dictionary<WaterParkCreature, float>();

        [HarmonyPrefix]
        public static void Prefix(WaterParkCreature __instance)
        {
            if((__instance.GetComponent<LiveMixin>()?.IsAlive() ?? false) && Main.Config.CreaturePowerGeneration.TryGetValue(__instance?.pickupable?.GetTechType() ?? TechType.None, out float powerValue))
            {
                if(!timeLastGenerated.TryGetValue(__instance, out float time))
                {
                    time = DayNightCycle.main.timePassedAsFloat;
                }

                float power = powerValue * (DayNightCycle.main.timePassedAsFloat - time) * Main.Config.PowerGenSpeed;
                PowerSource powerSource = __instance?.GetWaterPark()?.itemsRoot?.gameObject?.GetComponent<PowerSource>();

                if(powerSource != null)
                {
                    if(!powerSource.AddEnergy(power, out float amountStored))
                        powerSource.connectedRelay?.AddEnergy(power - amountStored, out _);
                }

                timeLastGenerated[__instance] = DayNightCycle.main.timePassedAsFloat;
            }
        }
    }
}
