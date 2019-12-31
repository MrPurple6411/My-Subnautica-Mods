using System;
using Harmony;
using WorldStreaming;

namespace IncreasedWorldLoad
{
    public static class Entry
    {
        public static void Patch()
        {
            HarmonyInstance.Create("MrPurple6411.IncreasedWorldLoad").PatchAll();
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
            __result.maxWorkspaces *= Environment.ProcessorCount;
            __result.maxMeshQueue *= Environment.ProcessorCount;

            for (int i = 1; i < __result.levels.Length; i++)
            {
                ClipMapManager.LevelSettings levelSettings = __result.levels[i];
                levelSettings.chunksPerSide = Environment.ProcessorCount;
                levelSettings.chunksVertically = Environment.ProcessorCount;
                levelSettings.grassSettings.reduction = 0;
                levelSettings.skipRelax = true;
                levelSettings.grass = true;
                levelSettings.entities = true;
            }
        }
    }
}