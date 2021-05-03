namespace ExtraOptions.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(WaterscapeVolume.Settings), nameof(WaterscapeVolume.Settings.GetExtinctionAndScatteringCoefficients))]
    public static class WaterscapeVolume_Settings_Patches
    {
        // Ref - https://forums.unknownworlds.com/discussion/154099/mod-pc-murky-waters-v2-with-dll-patcher-wip
        [HarmonyPrefix]
        public static bool Patch_GetExtinctionAndScatteringCoefficients(WaterscapeVolume.Settings __instance, ref Vector4 __result)
        {
            WaterscapeVolume.Settings t = __instance;
            float m = 1.0f - Mathf.Clamp(Main.Config.Murkiness / 400.0f, 0.0f, 1.0f);
            float mv = (m * 180.0f) + 10.0f;
            float d = t.murkiness / mv;
            Vector3 vector = t.absorption + (t.scattering * Vector3.one);
            __result = new Vector4(vector.x, vector.y, vector.z, t.scattering) * d;
            return false;
        }
    }
}