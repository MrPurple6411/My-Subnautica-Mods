using Harmony;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(PrecursorGunAim))]
    [HarmonyPatch("LateUpdate")]
    static class PrecursorGunAim_LateUpdate
    {
        [HarmonyPrefix]
        static bool Prefix(PrecursorGunAim __instance)
        {
            return !StoryGoalCustomEventHandler.main.gunDisabled; // Prevent the gun from aiming at the sunbeam if the gun is disabled
        }
    }
}
