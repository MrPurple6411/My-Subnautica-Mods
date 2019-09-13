using Harmony;
using Story;
using System;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(StoryGoal))]
    [HarmonyPatch("Execute")]
    static class StoryGoal_Execute
    {
        [HarmonyPrefix]
        static bool Prefix(StoryGoal __instance, string key, GoalType goalType)
        {
            if (key == "PrecursorGunAim") return false;
            ErrorMessage.AddDebug("GOAL COMPLETED: " + key + " (TYPE: " + goalType.ToString() + ")");
            Console.WriteLine("GOAL COMPLETED: " + key + " (TYPE: " + goalType.ToString() + ")");
            return true;
        }
    }
}
