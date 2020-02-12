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
        public static bool resetDefaults;
        public static bool chunkConfig;
        public static bool fragmentConfig;
        public static bool timeConfig;
        public static bool otherConfig;

        public static void Load()
        {
            SliderValue = PlayerPrefs.GetFloat("ResourceMultiplier", 1f);
            RegenSpawns = false;
            ToggleValue = PlayerPrefsExtra.GetBool("ResourceMultiplierEnabled", true);
            Randomization = PlayerPrefsExtra.GetBool("ResourceRandomizerEnabled", true);
            resetDefaults = false;
            chunkConfig = PlayerPrefsExtra.GetBool("chunkConfig", false);
            fragmentConfig = PlayerPrefsExtra.GetBool("fragmentConfig", false);
            timeConfig = PlayerPrefsExtra.GetBool("timeConfig", false);
            otherConfig = PlayerPrefsExtra.GetBool("otherConfig", false);
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
                    Config.techProbability[e.Id.SplitByChar(':')[0]] = (float)(e.Value);
                    PlayerPrefs.SetFloat(e.Id.SplitByChar(':')[0] + ":TechProbability", (float)(e.Value));
            }
        }
        public void ResourceOverloadOptions_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "ResourceMultiplierEnabled" && 
                e.Id != "RegenSpawnsEnabled" && 
                e.Id != "ResourceRandomizerEnabled" && 
                e.Id != "resetDefaults" &&
                e.Id != "chunkConfig" &&
                e.Id != "fragmentConfig" &&
                e.Id != "timeConfig" &&
                e.Id != "otherConfig") return;

            try
            {
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
                        IngameMenu.main.Close();
                        LargeWorldStreamer.main.cellManager.ResetEntityDistributions();
                        LargeWorldStreamer.main.ForceUnloadAll();
                        LargeWorldStreamer.main.cellManager.spawner.ResetSpawner();
                    }
                }
                if (e.Id == "ResourceRandomizerEnabled")
                {
                    Config.Randomization = e.Value;
                    PlayerPrefsExtra.SetBool("ResourceRandomizerEnabled", e.Value);
                    Config.RegenSpawns = true;
                    IngameMenu.main.Close();
                    LargeWorldStreamer.main.cellManager.ResetEntityDistributions();
                    LargeWorldStreamer.main.ForceUnloadAll();
                    LargeWorldStreamer.main.cellManager.spawner.ResetSpawner();
                }
                if (e.Id == "resetDefaults")
                {
                    Config.resetDefaults = e.Value;
                    PlayerPrefsExtra.SetBool("resetDefaults", e.Value);
                    if (Config.resetDefaults)
                    {
                        Config.chunkConfig = false;
                        PlayerPrefsExtra.SetBool("chunkConfig", false);
                        Config.fragmentConfig = false;
                        PlayerPrefsExtra.SetBool("fragmentConfig", false);
                        Config.timeConfig = false;
                        PlayerPrefsExtra.SetBool("timeConfig", false);
                        Config.otherConfig = false;
                        PlayerPrefsExtra.SetBool("otherConfig", false);
                        Config.RegenSpawns = true;
                        IngameMenu.main.Close();
                        LargeWorldStreamer.main.cellManager.ResetEntityDistributions();
                        LargeWorldStreamer.main.ForceUnloadAll();
                    }
                }
                if (e.Id == "chunkConfig")
                {
                    Config.fragmentConfig = false;
                    PlayerPrefsExtra.SetBool("fragmentConfig", false);
                    Config.timeConfig = false;
                    PlayerPrefsExtra.SetBool("timeConfig", false);
                    Config.otherConfig = false;
                    PlayerPrefsExtra.SetBool("otherConfig", false);
                    Config.chunkConfig = e.Value;
                    PlayerPrefsExtra.SetBool("chunkConfig", e.Value);
                    IngameMenu.main.Close();
                    IngameMenu.main.Open();
                    IngameMenu.main.ChangeSubscreen("Options");
                }
                if (e.Id == "fragmentConfig")
                {
                    Config.chunkConfig = false;
                    PlayerPrefsExtra.SetBool("chunkConfig", false);
                    Config.timeConfig = false;
                    PlayerPrefsExtra.SetBool("timeConfig", false);
                    Config.otherConfig = false;
                    PlayerPrefsExtra.SetBool("otherConfig", false);
                    Config.fragmentConfig = e.Value;
                    PlayerPrefsExtra.SetBool("fragmentConfig", e.Value);
                    IngameMenu.main.Close();
                    IngameMenu.main.Open();
                    IngameMenu.main.ChangeSubscreen("Options");
                }
                if (e.Id == "timeConfig")
                {
                    Config.chunkConfig = false;
                    PlayerPrefsExtra.SetBool("chunkConfig", false);
                    Config.fragmentConfig = false;
                    PlayerPrefsExtra.SetBool("fragmentConfig", false);
                    Config.otherConfig = false;
                    PlayerPrefsExtra.SetBool("otherConfig", false);
                    Config.timeConfig = e.Value;
                    PlayerPrefsExtra.SetBool("timeConfig", e.Value);
                    IngameMenu.main.Close();
                    IngameMenu.main.Open();
                    IngameMenu.main.ChangeSubscreen("Options");
                }
                if (e.Id == "otherConfig")
                {
                    Config.otherConfig = e.Value;
                    PlayerPrefsExtra.SetBool("otherConfig", e.Value);
                    Config.chunkConfig = false;
                    PlayerPrefsExtra.SetBool("chunkConfig", false);
                    Config.fragmentConfig = false;
                    PlayerPrefsExtra.SetBool("fragmentConfig", false);
                    Config.timeConfig = false;
                    PlayerPrefsExtra.SetBool("timeConfig", false);
                    IngameMenu.main.Close();
                    IngameMenu.main.Open();
                    IngameMenu.main.ChangeSubscreen("Options");

                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public override void BuildModOptions()
        {
            AddToggleOption("ResourceMultiplierEnabled", "Resource Multiplier Enabled", Config.ToggleValue);
            AddSliderOption("GeneralMultiplier", "Resource Multiplier", 0, 20, Config.SliderValue);
            AddToggleOption("ResourceRandomizerEnabled", "Resource Randomization Enabled", Config.Randomization);
            AddToggleOption("resetDefaults", "Reset Fine Tuning Configs", Config.resetDefaults);
            AddToggleOption("RegenSpawnsEnabled", "Apply changes?", Config.RegenSpawns);

            if (Config.Randomization)
            {
                AddToggleOption("chunkConfig", "Chunk Configs", Config.chunkConfig);
                foreach (string tech in Config.techProbability.Keys)
                {
                    if (tech.Trim().ToLower().Contains("chunk") && Config.chunkConfig)
                        AddSliderOption(tech + ":TechProbability", tech.SplitByChar(' ')[0], 0, 100, Config.techProbability[tech]);
                }

                AddToggleOption("fragmentConfig", "Fragment Configs", Config.fragmentConfig);
                foreach (string tech in Config.techProbability.Keys)
                {
                    if (tech.Trim().ToLower().Contains("fragment") && Config.fragmentConfig)
                        AddSliderOption(tech + ":TechProbability", tech, 0, 100, Config.techProbability[tech]);
                }

                AddToggleOption("timeConfig", "Time Configs", Config.timeConfig);
                foreach (string tech in Config.techProbability.Keys)
                {
                    if (tech.Trim().ToLower().Contains("time") && Config.timeConfig)
                        AddSliderOption(tech + ":TechProbability", tech, 0, 100, Config.techProbability[tech]);
                }

                AddToggleOption("otherConfig", "Other Configs", Config.otherConfig);
                foreach (string tech in Config.techProbability.Keys)
                {
                    if (Config.otherConfig && !tech.Trim().ToLower().Contains("reaper") && !tech.Trim().ToLower().Contains("chunk") && !tech.Trim().ToLower().Contains("fragment") && !tech.Trim().ToLower().Contains("time"))
                        AddSliderOption(tech + ":TechProbability", tech, 0, 100, Config.techProbability[tech]);
                }
            }
            else
            {
                AddToggleOption("chunkConfig", "Chunk Configs", Config.chunkConfig);
                foreach (string tech in Config.techProbability.Keys)
                {
                    if (tech.SplitByChar('|')[0].Trim().ToLower().Contains("chunk") && Config.chunkConfig)
                        AddSliderOption(tech + ":TechProbability", tech.SplitByChar('|')[0].SplitByChar(' ')[0] + ":" + tech.SplitByChar('|')[1], 0, 100, Config.techProbability[tech]);
                }

                AddToggleOption("fragmentConfig", "Fragment Configs", Config.fragmentConfig);
                foreach (string tech in Config.techProbability.Keys)
                {
                    if (tech.SplitByChar('|')[0].Trim().ToLower().Contains("fragment") && !tech.SplitByChar('|')[1].Trim().ToLower().Contains("fragment") && Config.fragmentConfig)
                        AddSliderOption(tech + ":TechProbability", tech.SplitByChar('|')[0] + ":" + tech.SplitByChar('|')[1], 0, 100, Config.techProbability[tech]);
                }

                AddToggleOption("timeConfig", "Time Configs", Config.timeConfig);
                foreach (string tech in Config.techProbability.Keys)
                {
                    if (tech.SplitByChar('|')[0].Trim().ToLower().Contains("time") && Config.timeConfig)
                        AddSliderOption(tech + ":TechProbability", tech.SplitByChar('|')[1], 0, 100, Config.techProbability[tech]);
                }

                AddToggleOption("otherConfig", "Other Configs", Config.otherConfig);
                foreach (string tech in Config.techProbability.Keys)
                {
                    if (Config.otherConfig && !tech.SplitByChar('|')[0].Trim().ToLower().Contains("reaper") && !tech.SplitByChar('|')[0].Trim().ToLower().Contains("chunk") && !tech.SplitByChar('|')[0].Trim().ToLower().Contains("fragment") && !tech.SplitByChar('|')[0].Trim().ToLower().Contains("time"))
                        AddSliderOption(tech + ":TechProbability", tech.SplitByChar('|')[0] + ":" + tech.SplitByChar('|')[1], 0, 100, Config.techProbability[tech]);
                }
            }
        }
    }
}
