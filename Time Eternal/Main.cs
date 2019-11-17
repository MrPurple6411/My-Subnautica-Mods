using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System;
using System.Reflection;
using UnityEngine;

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
    internal class Builder_Update_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(DayNightCycle __instance)
        {
            if (Time_Eternal.Config.OnOff)
            {
                if (Time_Eternal.Config.DayToggle)
                {
                    __instance.sunRiseTime = -1000.0f;
                    __instance.sunSetTime = 1000.0f;
                }
                else
                {
                    __instance.sunRiseTime = 1000.0f;
                    __instance.sunSetTime = -1000.0f;
                }

            }
            else
            {
                __instance.sunRiseTime = 0.125f;
                __instance.sunSetTime = 0.875f;
            }
            return true;
        }
    }

    public static class Config
    {
        public static bool OnOff;
        public static bool DayToggle;

        public static void Load()
        {
            DayToggle = PlayerPrefsExtra.GetBool("DayNightToggle", true);
            OnOff = PlayerPrefsExtra.GetBool("OnOff", true);
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Time Eternal")
        {
            ToggleChanged += Options_DayToggleChanged;
            ToggleChanged += Options_OnOffToggleChanged;
        }

        public void Options_DayToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "DayNightToggle") return;
            Config.DayToggle = e.Value;
            PlayerPrefsExtra.SetBool("DayNightToggle", e.Value);
        }

        public void Options_OnOffToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "OnOff") return;
            Config.OnOff = e.Value;
            PlayerPrefsExtra.SetBool("OnOff", e.Value);
        }

        public override void BuildModOptions()
        {
            AddToggleOption("OnOff", "Enabled", Config.OnOff);
            AddToggleOption("DayNightToggle", "Day or Night", Config.DayToggle);
        }
    }
}
