namespace BuilderModule.Patches
{
    using BuilderModule.Module;
    using HarmonyLib;
    using UWE;

    [HarmonyPatch]
    internal class LightToggle_Patches
    {
        [HarmonyPatch(typeof(ToggleLights), nameof(ToggleLights.SetLightsActive))]
        [HarmonyPrefix]
        private static bool Prefix(ToggleLights __instance)
        {
            BuilderModuleMono moduleMono = __instance.GetComponentInParent<BuilderModuleMono>();
            return moduleMono is null || !moduleMono.isToggle;
        }
    }
}