using System;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options;
using UnityEngine;
using WorldStreaming;

namespace WorldLoad
{
    public class Config: ConfigFile
    {
        public int IncreasedWorldLoad;

    }

    [QModCore]
    public static class Entry
    {
        internal static Config config = new Config();

        [QModPatch]
        public static void Patch()
        {
            config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());
            new Harmony("MrPurple6411.WorldLoad").PatchAll();
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("World Load Settings")
        {
            SliderChanged += WorldLoadOptions_SliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("WorldLoad", "Load Distance", 2, 50, Entry.config.IncreasedWorldLoad, 8);
        }

        public void WorldLoadOptions_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id == "WorldLoad")
            {
                Entry.config.IncreasedWorldLoad = e.IntegerValue;
            }
        }
    }

    [HarmonyPatch(typeof(WorldStreamer), "ParseClipmapSettings")]
    public class WorldStreamer_ParseClipmapSettings_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref ClipMapManager.Settings __result)
        {
            __result.maxThreads = Environment.ProcessorCount;
            __result.maxWorkspaces *= 4;
            __result.maxMeshQueue *= 4;

            for (int i = 1; i < __result.levels.Length; i++)
            {
                ClipMapManager.LevelSettings levelSettings = __result.levels[i];
                levelSettings.chunksPerSide = Entry.config.IncreasedWorldLoad;
                levelSettings.chunksVertically = Entry.config.IncreasedWorldLoad;
                levelSettings.grass = true;
                levelSettings.grassSettings.reduction = 0f;
            }
        }
    }
}