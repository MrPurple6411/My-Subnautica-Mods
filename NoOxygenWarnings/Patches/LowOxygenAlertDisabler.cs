namespace NoOxygenWarnings.Patches
{
    using HarmonyLib;

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