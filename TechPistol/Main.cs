#if !EDITOR
namespace TechPistol
{
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using System.IO;
    using System.Reflection;
    using Configuration;
    using Module;
    using UnityEngine;    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "TechPistol",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0",
            bundlePath =
#if SN1
            "Assets/TechPistol";
#elif BZ
            "Assets/TechPistolBZ";
#endif

        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);
        internal static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, bundlePath));
        internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        internal static PistolFragmentPrefab PistolFragment { get; } = new();
        internal static PistolPrefab Pistol { get; } = new();
        #endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

            PistolFragment.Patch();
            Pistol.Patch();
        }
    }
}
#endif