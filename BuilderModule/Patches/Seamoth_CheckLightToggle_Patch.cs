namespace BuilderModule.Patches
{
    using Module;
    using HarmonyLib;

    [HarmonyPatch]
    internal class LightToggle_Patches
    {
        [HarmonyPatch(typeof(ToggleLights), nameof(ToggleLights.SetLightsActive))]
        [HarmonyPrefix]
        private static bool Prefix(ToggleLights __instance)
        {
            var moduleMono = __instance.GetComponentInParent<BuilderModuleMono>();
            return moduleMono is null || !moduleMono.isToggle;
        }
    }
}