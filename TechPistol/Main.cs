using BepInEx;

#if !EDITOR
namespace TechPistol
{
    using HarmonyLib;
    using SMCLib.Handlers;
    using System.IO;
    using System.Reflection;
    using Configuration;
    using Module;
    using UnityEngine;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        private const string bundlePath = 
#if SN1
            "Assets/TechPistol";
#elif BZ
            "Assets/TechPistolBZ";
#endif
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);
        internal static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, bundlePath));
         internal static Config SmcConfig { get; private set; } 
        internal static PistolFragmentPrefab PistolFragment { get; } = new();
        internal static PistolPrefab Pistol { get; } = new();

        public void Start()
        {
            SmcConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            PistolFragment.Patch();
            Pistol.Patch();

            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}
#endif