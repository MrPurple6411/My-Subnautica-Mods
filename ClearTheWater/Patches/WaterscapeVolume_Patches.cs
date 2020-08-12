using HarmonyLib;
using UnityEngine;

namespace ClearTheWater.Patches
{
    [HarmonyPatch(typeof(WaterscapeVolume.Settings), nameof(WaterscapeVolume.Settings.GetExtinctionAndScatteringCoefficients))]
    internal class WaterscapeVolume_Settings_GetExtinctionAndScatteringCoefficients_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(WaterscapeVolume.Settings __instance, ref Vector4 __result)
        {
            Vector3 vector = __instance.absorption + (__instance.scattering * Vector3.one);
            __result = new Vector4(vector.x, vector.y, vector.z, __instance.scattering) * (__instance.murkiness / 400f);
            return false;
        }
    }

    [HarmonyPatch(typeof(WaterscapeVolume), "Awake")]
    internal class WaterscapeVolume_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(WaterscapeVolume __instance)
        {
            __instance.aboveWaterDensityScale = 1f;
        }
    }
}
