using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System;
using System.Reflection;
using UnityEngine;
using Story;

namespace Time_Eternal
{
    public static class Main
    {
        public static void Load()
        {
            Config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());
            try
            {
                HarmonyInstance.Create("MrPurple6411.Eternal_Sunshine").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    [HarmonyPatch(typeof(DayNightCycle))]
    [HarmonyPatch("GetDayNightCycleTime")]
    internal class DayNightCycle_GetDayNightCycleTime_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(DayNightCycle __instance)
        {
            if (Time_Eternal.Config.freezeTimeChoice == 1 && Time_Eternal.Config.timeEternalEnabled)
            {
                //always day
                __instance.sunRiseTime = -1000.0f;
                __instance.sunSetTime = 1000.0f;
                return true;
            }
            else if (Time_Eternal.Config.freezeTimeChoice == 2 && Time_Eternal.Config.timeEternalEnabled)
            {
                //always night
                __instance.sunRiseTime = 1000.0f;
                __instance.sunSetTime = -1000.0f;
                return true;
            }
            else if (Config.SpeedChoice == 0 && Config.timeEternalEnabled)
            {
                //10pm to 5:30am
                __instance.sunRiseTime = 0.2291666666666667f;
                __instance.sunSetTime = 0.9166666666666667f;
                return true;
            }
            else
            { 
                //9pm to 3am game default
                __instance.sunRiseTime = 0.125f;
                __instance.sunSetTime = 0.875f;
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(DayNightCycle))]
    [HarmonyPatch("GetDayScalar")]
    internal class DayNightCycle_GetDayScalar_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(DayNightCycle __instance, ref float __result)
        {
            System.DateTime epochStart = new System.DateTime(2019, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            if (Config.SpeedChoice != (3|0) && Time_Eternal.Config.timeEternalEnabled)
            {
                double num = UWE.Utils.Repeat(__instance.timePassed, 1200.0 * Time_Eternal.Config.getNewSpeed());
                float t = (float)(num / 1200.0 * Time_Eternal.Config.getNewSpeed());
                __result = Mathf.Repeat(t, 1f);
            }
            else if (Config.SpeedChoice == 0 && Time_Eternal.Config.timeEternalEnabled)
            {
                //links gametime to RL time.
                double num = UWE.Utils.Repeat(cur_time, 1200.0 * Time_Eternal.Config.getNewSpeed());
                float t = (float)(num / 1200.0 * Time_Eternal.Config.getNewSpeed());
                __result = Mathf.Repeat(t, 1f);
            }
            else
            {
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(DayNightCycle))]
    [HarmonyPatch("GetDay")]
    internal class DayNightCycle_GetDay_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(DayNightCycle __instance, ref double __result)
        {
            if (Config.SpeedChoice != 3 && Time_Eternal.Config.timeEternalEnabled)
            {
                __result = __instance.timePassed / 1200.0 * Time_Eternal.Config.getNewSpeed();
            }
            else
            {
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(StoryGoalScheduler))]
    [HarmonyPatch("Update")]
    internal class StoryGoalScheduler_Update_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(StoryGoalScheduler __instance)
        {
            if (Config.SpeedChoice == 3 | !Config.timeEternalEnabled)
            {
                for (int i = __instance.schedule.Count - 1; i >= 0; i--)
                {
                    ScheduledGoal scheduledGoal = __instance.schedule[i];
                    scheduledGoal.timeExecute = scheduledGoal.timeExecute / Config.SpeedMultiplier;
                }
                Config.SpeedMultiplier = 1f;
                return true;
            }
            else if (Time_Eternal.Config.timeEternalEnabled)
            {
                DayNightCycle main = DayNightCycle.main;
                if (!main)
                {
                    return true;
                }
                if (__instance.paused)
                {
                    return true;
                }
                for (int i = __instance.schedule.Count - 1; i >= 0; i--)
                {
                    ScheduledGoal scheduledGoal = __instance.schedule[i];
                    scheduledGoal.timeExecute = scheduledGoal.timeExecute / Config.SpeedMultiplier;
                    scheduledGoal.timeExecute = scheduledGoal.timeExecute * Config.getNewSpeed();
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public static class Config
    {
        public static int SpeedChoice;
        public static bool timeEternalEnabled;
        public static int freezeTimeChoice;
        public static float SpeedMultiplier;

        public static void Load()
        {
            SpeedChoice = PlayerPrefs.GetInt("DayNightSpeed", 3);
            freezeTimeChoice = PlayerPrefs.GetInt("DayNightToggle", 0);
            timeEternalEnabled = PlayerPrefsExtra.GetBool("Enabled", true);
        }

        public static float getNewSpeed()
        {
            if (SpeedChoice == 0)
            {
                SpeedMultiplier = 0.0139537863584958f;
            }
            if (SpeedChoice == 1)
            {
                SpeedMultiplier = 0.25f;
            }
            if (SpeedChoice == 2)
            {
                SpeedMultiplier = 0.5f;
            }
            if (SpeedChoice == 3)
            {
                SpeedMultiplier = 1f;
            }
            if (SpeedChoice == 4)
            {
                SpeedMultiplier = 2f;
            }
            if (SpeedChoice == 5)
            {
                SpeedMultiplier = 4f;
            }
            return SpeedMultiplier;
        }

    }

    public class Options : ModOptions
    {
        public Options() : base("Time Eternal")
        {
            ChoiceChanged += Options_ChoiceChanged;
            ChoiceChanged += Options_DayToggleChanged;
            ToggleChanged += Options_OnOffToggleChanged;
        }

        public void Options_ChoiceChanged(object sender, ChoiceChangedEventArgs e)
        {
            if (e.Id != "DayNightSpeedChoice") return;
            Config.SpeedChoice = e.Index;
            PlayerPrefs.SetInt("DayNightSpeed", e.Index);
        }
        public void Options_DayToggleChanged(object sender, ChoiceChangedEventArgs e)
        {
            if (e.Id != "DayNightToggle") return;
            Config.freezeTimeChoice = e.Index;
            PlayerPrefs.SetInt("DayNightToggle", e.Index);
        }

        public void Options_OnOffToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "Enabled") return;
            Config.timeEternalEnabled = e.Value;
            PlayerPrefsExtra.SetBool("Enabled", e.Value);
        }

        public override void BuildModOptions()
        {
            AddToggleOption("Enabled", "Enabled", Config.timeEternalEnabled);
            AddChoiceOption("DayNightToggle", "Freeze Lighting", new string[] { "Normal", "Day", "Night" }, Config.freezeTimeChoice);
            AddChoiceOption("DayNightSpeedChoice", "DayNightSpeed*", new string[] { "RealTime", "4x Longer", "2x Longer", "Normal", "2x Faster", "4x Faster" }, Config.SpeedChoice);
        }
    }
}
