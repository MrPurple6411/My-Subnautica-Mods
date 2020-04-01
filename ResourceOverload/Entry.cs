using Harmony;
using Oculus.Newtonsoft.Json;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ResourceOverload
{
    public static class Config
    {
        public static bool LoadExternalChanges;
        public static bool Randomization;
        public static bool RegenSpawns;
        public static bool resetDefaults;
        public static float ResourceMultiplier;
        public static SortedList<string, float> techProbability = new SortedList<string, float>();

        public static void DeleteCache()
        {
            string path = Randomization
                ? Path.GetDirectoryName(Assembly.GetAssembly(typeof(Entry)).Location) + "/RandomizerCache"
                : Path.GetDirectoryName(Assembly.GetAssembly(typeof(Entry)).Location) + "/Cache";

            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void Load()
        {
            ResourceMultiplier = PlayerPrefs.GetFloat("ResourceMultiplier", 1f);
            RegenSpawns = false;
            LoadExternalChanges = PlayerPrefsExtra.GetBool("LoadExternalChanges", false);
            Randomization = PlayerPrefsExtra.GetBool("ResourceRandomizerEnabled", true);
            resetDefaults = false;

            LoadCache();
            CustomLootDistributionData.LoadSettings();
            OptionsPanelHandler.RegisterModOptions(new Options());
        }

        public static void LoadCache()
        {
            string path = Randomization
                ? Path.GetDirectoryName(Assembly.GetAssembly(typeof(Entry)).Location) + "/RandomizerCache"
                : Path.GetDirectoryName(Assembly.GetAssembly(typeof(Entry)).Location) + "/Cache";

            if(File.Exists(path))
            {
                using(StreamReader reader = new StreamReader(path))
                {
                    CustomLootDistributionData.customDSTDistribution = JsonConvert.DeserializeObject<SortedDictionary<BiomeType, LootDistributionData.DstData>>(reader.ReadToEnd());
                }
            }
        }
    }

    public class Entry
    {
        public static void Load()
        {
            Config.Load();
            HarmonyInstance harmony = HarmonyInstance.Create("MrPurple6411.ResourceOverload");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public class Options: ModOptions
    {
        public Options() : base("******Resource Overload Configuration******")
        {
            SliderChanged += ResourceOverloadOptions_SliderChanged;
            ToggleChanged += ResourceOverloadOptions_ToggleChanged;
        }

        public override void BuildModOptions()
        {
            AddToggleOption("LoadExternalChanges", "Load External Changes", Config.LoadExternalChanges);
            AddSliderOption("ResourceMultiplier", "Resource Multiplier", 1, 20, Config.ResourceMultiplier);
            AddToggleOption("RegenSpawnsEnabled", "Apply Multiplier Changes", Config.RegenSpawns);
            AddToggleOption("ResourceRandomizerEnabled", "Resource Randomization Enabled", Config.Randomization);
            AddToggleOption("resetDefaults", "Reset Current Config", Config.resetDefaults);
        }

        public void ResourceOverloadOptions_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if(e.Id != "ResourceMultiplier" &&
                !e.Id.Contains("TechProbability"))
            {
                return;
            }

            if(e.Id == "ResourceMultiplier")
            {
                Config.ResourceMultiplier = e.Value;
                PlayerPrefs.SetFloat("ResourceMultiplier", e.Value);
            }
            else if(e.Id.Contains(":TechProbability"))
            {
                Config.techProbability[e.Id.SplitByChar(':')[0]] = e.Value;
                PlayerPrefs.SetFloat(e.Id.SplitByChar(':')[0] + ":TechProbability", e.Value);
                CustomLootDistributionData.changed = true;
            }
        }

        public void ResourceOverloadOptions_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if(e.Id != "LoadExternalChanges" &&
                e.Id != "RegenSpawnsEnabled" &&
                e.Id != "ResourceRandomizerEnabled" &&
                e.Id != "resetDefaults")
            {
                return;
            }

            try
            {
                if(e.Id == "LoadExternalChanges")
                {
                    Config.DeleteCache();
                    Config.techProbability = new SortedList<string, float>();
                    CustomLootDistributionData.customDSTDistribution = new SortedDictionary<BiomeType, LootDistributionData.DstData>();
                    Config.RegenSpawns = e.Value;
                    IngameMenu.main.Close();
                    LargeWorldStreamer.main.cellManager.ResetEntityDistributions();
                    LargeWorldStreamer.main.ForceUnloadAll();
                    LargeWorldStreamer.main.cellManager.spawner.ResetSpawner();
                }
                if(e.Id == "RegenSpawnsEnabled")
                {
                    Config.RegenSpawns = e.Value;
                    PlayerPrefsExtra.SetBool("RegenSpawnsEnabled", e.Value);
                    if(e.Value)
                    {
                        IngameMenu.main.Close();
                        LargeWorldStreamer.main.cellManager.ResetEntityDistributions();
                        LargeWorldStreamer.main.ForceUnloadAll();
                        LargeWorldStreamer.main.cellManager.spawner.ResetSpawner();
                    }
                }
                if(e.Id == "ResourceRandomizerEnabled")
                {
                    Config.Randomization = e.Value;
                    PlayerPrefsExtra.SetBool("ResourceRandomizerEnabled", e.Value);
                    Config.RegenSpawns = true;
                    Config.techProbability = new SortedList<string, float>();
                    CustomLootDistributionData.customDSTDistribution = new SortedDictionary<BiomeType, LootDistributionData.DstData>();
                    Config.LoadCache();
                    IngameMenu.main.Close();
                    LargeWorldStreamer.main.cellManager.ResetEntityDistributions();
                    LargeWorldStreamer.main.ForceUnloadAll();
                    LargeWorldStreamer.main.cellManager.spawner.ResetSpawner();
                }
                if(e.Id == "resetDefaults")
                {
                    Config.resetDefaults = e.Value;
                    PlayerPrefsExtra.SetBool("resetDefaults", e.Value);
                    if(Config.resetDefaults)
                    {
                        Config.RegenSpawns = true;
                        IngameMenu.main.Close();
                        LargeWorldStreamer.main.cellManager.ResetEntityDistributions();
                        LargeWorldStreamer.main.ForceUnloadAll();
                    }
                }
            }
            catch(Exception)
            {
                return;
            }
        }
    }
}