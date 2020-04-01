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
            public static bool Prefix(PrecursorGunAim __instance)
            {
                if(StoryGoalCustomEventHandler.main.gunDisabled)
                {
                    return false;
                }
                return true;
            }
        }
    }
}