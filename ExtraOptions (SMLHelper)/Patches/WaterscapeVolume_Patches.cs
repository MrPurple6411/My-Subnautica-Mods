using HarmonyLib;

namespace ExtraOptions.Patches
{
    [HarmonyPatch(typeof(WaterscapeVolume), nameof(WaterscapeVolume.RenderImage))]
    public static class WaterscapeVolume_Patches
    {
        [HarmonyPrefix]
        public static void Patch_RenderImage(ref bool cameraInside)
        {
            if (Main.config.FogFix)
                cameraInside = false;
        }
    }

    [HarmonyPatch(typeof(WaterscapeVolume), nameof(WaterscapeVolume.PreRender))]
    internal class WaterscapeVolume_PreRender_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(WaterscapeVolume __instance)
        {
            if(Main.config.ClearSurface)
                __instance.aboveWaterDensityScale = 1f;
            else
                __instance.aboveWaterDensityScale = 10f;

        }
    }
}