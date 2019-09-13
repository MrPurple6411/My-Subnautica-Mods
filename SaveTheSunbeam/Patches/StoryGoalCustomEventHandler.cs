using Harmony;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(StoryGoalCustomEventHandler))]
    [HarmonyPatch("NotifyGoalComplete")]
    static class StoryGoalCustomEventHandler_NotifyGoalComplete
    {
        [HarmonyPrefix]
        static void Prefix(StoryGoalCustomEventHandler __instance, ref string key)
        {
            if (key.ToLower() == "radiosunbeamstart" || key.ToLower() == "sunbeamcheckplayerrange")
                if (__instance.gunDisabled)
                    key = "nope";
        }
    }
}
