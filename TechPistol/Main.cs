#if !EDITOR
namespace TechPistol
{
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using System.IO;
    using System.Reflection;
    using Configuration;
    using Module;
    using UnityEngine;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
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
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static PistolFragmentPrefab PistolFragment { get; } = new();
        internal static PistolPrefab Pistol { get; } = new();

        #region[Declarations]

        public const string
            MODNAME = "TechPistol",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        #endregion

        private void Awake()
        {
            PistolFragment.Patch();
            Pistol.Patch();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}
#endif