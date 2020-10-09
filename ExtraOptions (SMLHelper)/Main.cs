using System;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using System.IO;
using QModManager.API.ModLoading;
using BiomeSettings = WaterBiomeManager.BiomeSettings;
using Logger = QModManager.Utility.Logger;
using ExtraOptions.Configuration;
using SMLHelper.V2.Handlers;
#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

namespace ExtraOptions
{
    [QModCore]
    public static class Main
    {
        internal static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static readonly string modpath = Path.GetDirectoryName(assembly.Location);
        internal static readonly string themesPath = $"{modpath}/theme.json";
        internal static WaterBiomeManager cachedWBM;
        internal static BiomeSettings cachedBiomeSettings;
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            Harmony harmony = Harmony.CreateAndPatchAll(assembly, "com.m22spencer.extraoptions");

            // When a preset is selected, the texture quality is also set, reload settings here to override this
            harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.SyncQualityPresetSelection)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
            harmony.Patch(AccessTools.Method(typeof(MainMenuController), nameof(MainMenuController.Start)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
        }

        public static void ApplyOptions()
        {
            try
            {
                foreach (var w in UnityEngine.Object.FindObjectsOfType<WaterBiomeManager>())
                    w.Rebuild();

                QualitySettings.masterTextureLimit = 4 - config.TextureQuality;

                foreach (var s in UnityEngine.Object.FindObjectsOfType<WaterSunShaftsOnCamera>())
                    s.enabled = config.LightShafts;

                if (!config.VariablePhysicsStep)
                {
                    Time.fixedDeltaTime = 0.02f;
                    Time.maximumDeltaTime = 0.33333f;
                    Time.maximumParticleDeltaTime = 0.03f;
                }

                config.Save();
            }
            catch (Exception e)
            {
                Logger.Log(Logger.Level.Error, msg: "Reload failed with Exception: \n", ex: e);
            }
        }

        public static BiomeSettings GetBiome()
        {
            var pos = Player.main?.gameObject.transform.position;

            if (pos != null)
            {
                if (cachedWBM is null)
                {
                    cachedWBM = UnityEngine.Object.FindObjectOfType<WaterBiomeManager>();
                    cachedBiomeSettings = null;
                }

                string biome = cachedWBM?.GetBiome(pos.Value);

                if (cachedBiomeSettings is null || cachedBiomeSettings?.name != biome)
                {
                    cachedBiomeSettings = cachedWBM?.biomeSettings?.FirstOrDefault(b => b.name == biome);
                    if(cachedBiomeSettings != null)
                        Logger.Log(Logger.Level.Debug, $"Entering new Biome: {biome}", showOnScreen: true);
                }

                return cachedBiomeSettings;
            }
            return null;
        }

        public static JsonSerializerSettings themeJSS = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Converters = new JsonConverter[]
            {
                new ColorConverter(),
                new Vector3Converter()
            }
        };
    }
}