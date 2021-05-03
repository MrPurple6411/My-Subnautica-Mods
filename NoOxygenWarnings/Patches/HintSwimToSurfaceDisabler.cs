namespace NoOxygenWarnings.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(HintSwimToSurface), nameof(HintSwimToSurface.ShouldShowWarning))]
    internal class HintSwimToSurfaceDisabler
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}