using System;
using Harmony;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using UnityEngine;
using WorldStreaming;

namespace WorldLoad
{
    public static class Config
    {
        public static int IncreasedWorldLoad;

        public static void Load()
        {
            IncreasedWorldLoad = PlayerPrefs.GetInt("WorldLoad", 8);
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

    [QModCore]
    public static class Entry
    {
        [QModPatch]
        public static void Patch()
        {
            Config.Load();
            HarmonyInstance.Create("MrPurple6411.WorldLoad").PatchAll();
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("World Load Settings")
        {
            SliderChanged += ResourceOverloadOptions_SliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("WorldLoad", "Load Distance", 2, 50, Config.IncreasedWorldLoad);
        }

        public void ResourceOverloadOptions_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "WorldLoad")
            {
                return;
            }

            Config.IncreasedWorldLoad = (int)e.Value;
            PlayerPrefs.SetInt("WorldLoad", (int)e.Value);
        }
    }

    [HarmonyPatch(typeof(WorldStreamer))]
    [HarmonyPatch(nameof(WorldStreamer.ParseClipmapSettings))]
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
                levelSettings.chunksPerSide = Config.IncreasedWorldLoad;
                levelSettings.chunksVertically = Config.IncreasedWorldLoad;
            }
        }
    }
}