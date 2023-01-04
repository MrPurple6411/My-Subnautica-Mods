namespace ExtraOptions
{
    using Configuration;
    using HarmonyLib;
    using SMLHelper.Handlers;
    using System.IO;
    using System.Reflection;
    using UnityEngine;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "ExtraOptions",
            AUTHOR = "m22spencer-MrPurple6411",
            GUID = "com.m22spencer.extraOptions",
            VERSION = "1.0.0.0";
        internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        #endregion

        private void Awake()
        {
            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

            // When a preset is selected, the texture quality is also set, reload settings here to override this
            harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.SyncQualityPresetSelection)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
            harmony.Patch(AccessTools.Method(typeof(Player), nameof(Player.Start)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
            harmony.Patch(AccessTools.Method(typeof(MainMenuController), nameof(MainMenuController.Start)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
        }

        public static void ApplyOptions()
        {
            try
            {

                QualitySettings.masterTextureLimit = 4 - SMLConfig.TextureQuality;

                switch(SMLConfig.ShadowCascades)
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

                Shader.globalMaximumLOD = SMLConfig.ShaderLOD;
                QualitySettings.lodBias = SMLConfig.LODGroupBias;

                foreach(var s in Object.FindObjectsOfType<WaterSunShaftsOnCamera>() ?? new WaterSunShaftsOnCamera[0])
                    s.enabled = SMLConfig.LightShafts;

                foreach(var p in Object.FindObjectsOfType<AmbientParticles>() ?? new AmbientParticles[0])
                    p.enabled = SMLConfig.AmbientParticles;

                if(!SMLConfig.VariablePhysicsStep)
                {
                    Time.fixedDeltaTime = 0.02f;
                    Time.maximumDeltaTime = 0.33333f;
                    Time.maximumParticleDeltaTime = 0.03f;
                }

                foreach(var w in Object.FindObjectsOfType<WaterBiomeManager>() ?? new WaterBiomeManager[0])
                    w.Rebuild();

                SMLConfig.Save();
            }
            catch
            {
                // ignored
            }
        }

    }
}