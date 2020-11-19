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
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            Harmony harmony = Harmony.CreateAndPatchAll(assembly, "com.m22spencer.extraoptions");

            // When a preset is selected, the texture quality is also set, reload settings here to override this
            harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.SyncQualityPresetSelection)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
            harmony.Patch(AccessTools.Method(typeof(Player), nameof(Player.Start)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
            harmony.Patch(AccessTools.Method(typeof(MainMenuController), nameof(MainMenuController.Start)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
        }

        public static void ApplyOptions()
        {
            try
            {

                QualitySettings.masterTextureLimit = 4 - config.TextureQuality;

                switch (config.ShadowCascades)
                {
                    case 0:
                        QualitySettings.shadowCascades = 1;
                        break;
                    case 1:
                        QualitySettings.shadowCascades = 2;
                        break;
                    case 2:
                        QualitySettings.shadowCascades = 4;
                        break;
                }

                Shader.globalMaximumLOD = config.ShaderLOD;
                QualitySettings.lodBias = config.LODGroupBias;

                foreach (WaterSunShaftsOnCamera s in UnityEngine.Object.FindObjectsOfType<WaterSunShaftsOnCamera>())
                    s.enabled = config.LightShafts;
                
                foreach (AmbientParticles p in UnityEngine.Object.FindObjectsOfType<AmbientParticles>())
                    p.enabled = config.AmbientParticles;

                if (!config.VariablePhysicsStep)
                {
                    Time.fixedDeltaTime = 0.02f;
                    Time.maximumDeltaTime = 0.33333f;
                    Time.maximumParticleDeltaTime = 0.03f;
                }

                foreach (WaterBiomeManager w in UnityEngine.Object.FindObjectsOfType<WaterBiomeManager>())
                    w.Rebuild();

                config.Save();
            }
            catch (Exception e)
            {
                Logger.Log(Logger.Level.Error, msg: "Reload failed with Exception: \n", ex: e);
            }
        }

    }
}