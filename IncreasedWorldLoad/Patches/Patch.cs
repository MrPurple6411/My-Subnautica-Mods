using HarmonyLib;
using System;
using System.Linq;
using WorldStreaming;

namespace IncreasedWorldLoad.Patches
{

    [HarmonyPatch(typeof(WorldStreamer), nameof(WorldStreamer.ParseClipmapSettings))]
    public static class ParseClipmapSettings_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref ClipMapManager.Settings __result)
        {
            __result.maxThreads = Environment.ProcessorCount;
            __result.maxWorkspaces += __result.maxWorkspaces;

            for(int i = 2; i< __result.levels.Length;i++)
            {
                var level = __result.levels[i];
                level.chunksPerSide += i==__result.levels.Length-1? __result.levels.Sum((x)=>x.chunksPerSide) : level.chunksPerSide;
                level.chunksVertically = i == __result.levels.Length - 1 ? __result.levels.Sum((x) => x.chunksPerSide) : level.chunksVertically;
                level.entities = true;
                level.grass = true;
            }
        }
    }
}
