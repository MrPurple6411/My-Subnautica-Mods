using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace ResourceOverload
{
    public class Entry
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.ResourceOverload");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }
    public static class Config
    {
        public static float SliderValue;
        public static bool ToggleValue;
        public static SortedList<string, float> techProbability = new SortedList<string, float>();
        public static bool RegenSpawns;
        public static bool Randomization;
        public static bool ShowConfig;
        public static bool resetDefaults;

        public static void Load()
        {
            SliderValue = PlayerPrefs.GetFloat("ResourceMultiplier", 1f);
            RegenSpawns = false;
            ToggleValue = PlayerPrefsExtra.GetBool("ResourceMultiplierEnabled", true);
            ShowConfig = false;
            Randomization = PlayerPrefsExtra.GetBool("ResourceRandomizerEnabled", true);
            resetDefaults = false;
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("******Resource Overload Configuration******")
        {
            SliderChanged += ResourceOverloadOptions_SliderChanged;
            ToggleChanged += ResourceOverloadOptions_ToggleChanged;
        }

        public void ResourceOverloadOptions_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "GeneralMultiplier" &&
                !e.Id.Contains("TechProbability")) return;
            if (e.Id == "GeneralMultiplier")
            {
                Config.SliderValue = e.Value;
                PlayerPrefs.SetFloat("ResourceMultiplier", e.Value);
            }
            else if (e.Id.Contains(":TechProbability"))
            {
                    Config.techProbability[e.Id.Split(':')[0]] = (float)(e.Value);
                    PlayerPrefs.SetFloat(e.Id.Split(':')[0] + ":TechProbability", (float)(e.Value));
            }
        }
        public void ResourceOverloadOptions_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "ResourceMultiplierEnabled" && e.Id != "RegenSpawnsEnabled" && e.Id != "ResourceRandomizerEnabled" && e.Id != "ShowConfig" && e.Id != "resetDefaults") return;
            if (e.Id == "ResourceMultiplierEnabled")
            {
                Config.ToggleValue = e.Value;
                PlayerPrefsExtra.SetBool("ResourceMultiplierEnabled", e.Value);
            }
            if (e.Id == "RegenSpawnsEnabled")
            {
                Config.RegenSpawns = e.Value;
                PlayerPrefsExtra.SetBool("RegenSpawnsEnabled", e.Value);
                if (e.Value)
                {
                    Config.ShowConfig = false;
                    IngameMenu.main.Close();
                    DevConsole.SendConsoleCommand("entreset");
                }
            }
            if (e.Id == "ShowConfig")
            {
                Config.ShowConfig = e.Value;
                PlayerPrefsExtra.SetBool("ShowConfig", e.Value);
                IngameMenu.main.Close();
                IngameMenu.main.Open();
                IngameMenu.main.ChangeSubscreen("Options");
            }
            if (e.Id == "ResourceRandomizerEnabled")
            {
                Config.Randomization = e.Value;
                PlayerPrefsExtra.SetBool("ResourceRandomizerEnabled", e.Value);
                Config.ShowConfig = false;
                Config.RegenSpawns = true;
                IngameMenu.main.Close();
                DevConsole.SendConsoleCommand("entreset");
            }
            if (e.Id == "resetDefaults")
            {
                Config.resetDefaults = e.Value;
                PlayerPrefsExtra.SetBool("resetDefaults", e.Value);
                if (Config.resetDefaults)
                {
                    Config.ShowConfig = false;
                    Config.RegenSpawns = true;
                    IngameMenu.main.Close();
                    DevConsole.SendConsoleCommand("entreset");
                }
            }
        }

        public override void BuildModOptions()
        {
            AddToggleOption("ResourceMultiplierEnabled", "Resource Multiplier Enabled", Config.ToggleValue);
            AddSliderOption("GeneralMultiplier", "Resource Multiplier", 0, 20, Config.SliderValue);
            AddToggleOption("ResourceRandomizerEnabled", "Resource Randomization Enabled", Config.Randomization);
            AddToggleOption("ShowConfig", "Show Fine Tuning Configs", Config.ShowConfig);
            AddToggleOption("resetDefaults", "Reset Fine Tuning Configs", Config.resetDefaults);
            AddToggleOption("RegenSpawnsEnabled", "Apply changes?", Config.RegenSpawns);
            if (Config.ShowConfig)
            {
                foreach (string tech in Config.techProbability.Keys)
                {
                    AddSliderOption(tech + ":TechProbability", tech.Split('|')[0].Trim() + ":" + tech.Split('|')[1].Trim(), 0, 100, Config.techProbability[tech]);
                }
            }
        }
    }
}
