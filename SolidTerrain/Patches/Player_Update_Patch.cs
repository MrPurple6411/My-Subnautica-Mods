namespace SolidTerrain.Patches;

using WorldStreaming;
using HarmonyLib;

[HarmonyPatch]
public class Patches
{
    [HarmonyPatch(typeof(WorldStreamer), nameof(WorldStreamer.ParseClipmapSettings))]
    public static void Postfix(ref ClipMapManager.Settings __result)
    {
        var levels = __result.levels;

        for (int i = 1; i < __result.levels.Length-2; i++)
        {
            var level = levels[i];

            if (level.entities)
            {
                level.downsamples = levels[0].downsamples;
                level.colliders = true;
                level.grass = true;
                level.grassSettings = levels[0].grassSettings;
                levels[i] = level;
            }
        }

        __result.levels = levels;
    }
}