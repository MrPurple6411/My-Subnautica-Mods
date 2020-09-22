using HarmonyLib;
using UnityEngine;

namespace BetterACU.Patches
{
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
    [HarmonyPatch(typeof(WaterParkCreature), "Update")]
#elif BZ
    [HarmonyPatch(typeof(WaterParkCreature), "ManagedUpdate")]
#endif
    internal class WaterParkCreature_Update_Prefix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterParkCreature __instance)
        {
            if (Main.config.CreaturePowerGeneration.TryGetValue(__instance?.pickupable?.GetTechType() ?? TechType.None, out float powerValue))
            {
                float power = powerValue/10 * Time.deltaTime * Main.config.PowerGenSpeed;
                __instance?.GetWaterPark()?.gameObject?.GetComponent<PowerSource>()?.AddEnergy(power, out _);
            }
        }
    }
}
