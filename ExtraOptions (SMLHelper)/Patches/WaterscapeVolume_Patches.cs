namespace ExtraOptions.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(WaterscapeVolume), nameof(WaterscapeVolume.RenderImage))]
    public static class WaterscapeVolume_Patches
    {
        [HarmonyPrefix]
        public static void Patch_RenderImage(ref bool cameraInside)
        {
            if(Main.Config.FogFix)
                cameraInside = false;
        }
    }

    [HarmonyPatch(typeof(WaterscapeVolume), nameof(WaterscapeVolume.PreRender))]
    internal class WaterscapeVolume_PreRender_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(WaterscapeVolume __instance)
        {
            __instance.aboveWaterDensityScale = Main.Config.ClearSurface ? 1f : 10f;

        }
    }
}