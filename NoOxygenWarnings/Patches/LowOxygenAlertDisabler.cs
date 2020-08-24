using HarmonyLib;

namespace NoOxygenWarnings.Patches
{
    [HarmonyPatch(typeof(LowOxygenAlert), nameof(LowOxygenAlert.Update))]
    internal class LowOxygenAlertDisabler
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}