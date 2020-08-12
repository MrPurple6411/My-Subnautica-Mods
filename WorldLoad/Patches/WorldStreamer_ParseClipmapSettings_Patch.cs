using System;
using HarmonyLib;
using WorldStreaming;

namespace WorldLoad.Patches
{
    [HarmonyPatch(typeof(WorldStreamer), "ParseClipmapSettings")]
    public class WorldStreamer_ParseClipmapSettings_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref ClipMapManager.Settings __result)
        {
            __result.maxThreads = Environment.ProcessorCount;
            __result.maxWorkspaces *= 4;
            __result.maxMeshQueue *= 4;

            for (int i = 1; i < __result.levels.Length-1; i++)
            {
                ClipMapManager.LevelSettings levelSettings = __result.levels[i];
                levelSettings.chunksPerSide = Main.config.IncreasedWorldLoad;
                levelSettings.chunksVertically = Main.config.IncreasedWorldLoad;
                levelSettings.grass = true;
                levelSettings.grassSettings.reduction = 0f;
            }
        }
    }
}