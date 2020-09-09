using System.Collections.Generic;
using HarmonyLib;
using System.IO;
#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

namespace ExtraOptions.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class Player_Patches
    {
        public static string inBiome;

        [HarmonyPrefix]
        public static void Patch_PlayerUpdate(Player __instance)
        {
            var biome = Main.GetBiome();
            if (biome != null && biome.name != inBiome && File.Exists(Main.themesPath))
            {
                inBiome = biome.name;
                var themes = JsonConvert.DeserializeObject<Dictionary<string, WaterscapeVolume.Settings>>(File.ReadAllText(Main.themesPath), Main.themeJSS);
                if (themes.TryGetValue(biome.name, out var theme))
                {
                    biome.settings = theme;
                    Main.ApplyOptions();
                }
            }
        }
    }
}