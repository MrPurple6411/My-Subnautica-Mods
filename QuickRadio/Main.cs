using Harmony;
using System;
using System.Reflection;
using UnityEngine;
using Story;

namespace QuickRadio
{
    public class Main
    {
        public static void Load()
        {
            try
            {
                HarmonyInstance.Create("MrPurple6411.QuickRadio").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    [HarmonyPatch(typeof(StoryGoalScheduler))]
    [HarmonyPatch("Update")]
    internal class StoryGoalScheduler_Update_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(StoryGoalScheduler __instance)
        {
            StoryGoalManager.main.PulsePendingMessages();
            for (int i = __instance.schedule.Count - 1; i >= 0; i--)
            {
                ScheduledGoal scheduledGoal = __instance.schedule[i];
                if (scheduledGoal.goalType == Story.GoalType.Radio)
                {
                    __instance.schedule[i] = __instance.schedule[__instance.schedule.Count - 1];
                    __instance.schedule.RemoveAt(__instance.schedule.Count - 1);
                    StoryGoal.Execute(scheduledGoal.goalKey, scheduledGoal.goalType);
                }
            }
            return true;
        }
    }
}
