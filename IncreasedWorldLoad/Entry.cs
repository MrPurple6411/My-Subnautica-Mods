using System;
using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using UnityEngine;
using WorldStreaming;

namespace WorldLoad
{
    public static class Entry
    {
        public static void Patch()
        {
            HarmonyInstance.Create("MrPurple6411.WorldLoad").PatchAll();
            Config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

    public static class Config
    {
        public static int IncreasedWorldLoad;

        public static void Load()
        {
            IncreasedWorldLoad = PlayerPrefs.GetInt("WorldLoad", 8);
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("World Load Settings")
        {
            SliderChanged += ResourceOverloadOptions_SliderChanged;
        }

        public void ResourceOverloadOptions_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "WorldLoad") return;
            Config.IncreasedWorldLoad = (int)e.Value;
            PlayerPrefs.SetInt("WorldLoad", (int)e.Value);
        }

        public override void BuildModOptions()
        {
            AddSliderOption("WorldLoad", "Load Distance", 1, 20, Config.IncreasedWorldLoad);
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
            __result.maxWorkspaces *= Config.IncreasedWorldLoad/4;
            __result.maxMeshQueue *= Config.IncreasedWorldLoad/4;

            for (int i = 1; i < __result.levels.Length; i++)
            {
                ClipMapManager.LevelSettings levelSettings = __result.levels[i];
                levelSettings.chunksPerSide = Config.IncreasedWorldLoad/i;
                levelSettings.chunksVertically = Config.IncreasedWorldLoad/i;
                levelSettings.grassSettings.reduction = 0;
                levelSettings.grass = true;
                levelSettings.entities = true;
            }
        }
    }
}