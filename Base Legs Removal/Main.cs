namespace BaseLegsRemoval;

using Configuration;
using HarmonyLib;
using System.Reflection;
using Nautilus.Handlers;
using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main: BaseUnityPlugin
{
    internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();

    private void Awake()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
		Logger.LogInfo("Loaded " + MyPluginInfo.PLUGIN_NAME);
	}
}