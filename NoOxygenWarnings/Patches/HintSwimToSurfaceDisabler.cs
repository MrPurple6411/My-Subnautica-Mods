using HarmonyLib;

namespace NoOxygenWarnings.Patches
{
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