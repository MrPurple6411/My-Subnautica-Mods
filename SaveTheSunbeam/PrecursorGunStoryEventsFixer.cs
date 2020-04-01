using Harmony;

namespace SaveTheSunbeam
{
    internal class PrecursorGunAimFixer
    {
        [HarmonyPatch(typeof(PrecursorGunAim))]
        [HarmonyPatch("LateUpdate")]
        internal class LateUpdateFixer
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                return !StoryGoalCustomEventHandler.main.gunDisabled;
            }
        }
    }
}