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
    internal class WaterParkCreature_Update_Prefix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterParkCreature __instance)
        {
            if (__instance?.pickupable?.GetTechType() == TechType.Shocker && (__instance?.GetCanBreed() ?? false) && DayNightCycle.main?.timePassed > __instance?.timeNextBreed)
            {
                __instance?.GetWaterPark()?.gameObject?.GetComponent<PowerSource>()?.AddEnergy(100f, out _);
            }
        }
    }

#elif BZ
    
    [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.ManagedUpdate))]
    internal class WaterParkCreature_ManagedUpdate_Prefix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterParkCreature __instance)
        {
            if(__instance.pickupable.GetTechType() == TechType.Jellyfish && __instance.GetCanBreed() && DayNightCycle.main.timePassed > (double)__instance.timeNextBreed)
            {
                __instance.GetWaterPark()?.gameObject.GetComponent<PowerSource>()?.AddEnergy(100f, out float stored);
            }
        }
    }
#endif
}
