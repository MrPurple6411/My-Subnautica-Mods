using Harmony;
using Story;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(StoryGoal))]
    [HarmonyPatch("Execute")]
    static class StoryGoal_Execute
    {
        [HarmonyPrefix]
        static bool Prefix(StoryGoal __instance, string key, GoalType goalType)
        {
            if (key.ToLower() == "precursorgunaim") return false;
            return true;
        }
    }
}
