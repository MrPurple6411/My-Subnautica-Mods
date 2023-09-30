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
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    public const string
        bundlePath =
#if SUBNAUTICA
        "Assets/TechPistol";
#elif BELOWZERO
        "Assets/TechPistolBZ";
#endif

    private static Assembly assembly = Assembly.GetExecutingAssembly();
    private static string modPath = Path.GetDirectoryName(assembly.Location);
    internal static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, bundlePath));
    internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
    internal static PistolFragmentPrefab PistolFragment { get; } = new();
    internal static PistolPrefab Pistol { get; } = new();

    private void Awake()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

        PistolFragment.Patch();
        Pistol.Patch();
    }
}
#endif