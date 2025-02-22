namespace BuildingTweaks;

using Configuration;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;using BepInEx;
using BepInEx.Logging;

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
    public static new SMLConfig Config { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
    internal static new ManualLogSource Logger { get; private set; }

    private void Awake()
    {
		Logger = base.Logger;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
    }
}