#if !EDITOR
namespace TechPistol;

using HarmonyLib;
using Nautilus.Handlers;
using System.IO;
using System.Reflection;
using Configuration;
using Module;
using UnityEngine;using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main: BaseUnityPlugin
{
    public const string
        bundlePath =
#if SUBNAUTICA
        "Assets/TechPistol";
#elif BELOWZERO
        "Assets/TechPistolBZ";
#endif

    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private static readonly string _modPath = Path.GetDirectoryName(_assembly.Location);
    internal static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(_modPath, bundlePath));
    internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
    internal static PistolFragmentPrefab PistolFragment { get; private set; }
    internal static PistolPrefab Pistol { get; private set; }

    private void Awake()
    {
		PistolFragment = new();
		PistolFragment.Prefab.Register();

		Pistol = new();
        Pistol.Prefab.Register();

		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
		Logger.LogInfo($"TechPistol loaded");
	}
}
#endif