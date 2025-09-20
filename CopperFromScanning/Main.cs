namespace CopperFromScanning;

using System.Reflection;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using CopperFromScanning.Configuration;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main: BaseUnityPlugin
{
    public static Config ModConfig { get; private set; }

    private void Awake()
    {
    // Register Nautilus config/options menu and load defaults
        ModConfig = OptionsPanelHandler.RegisterModOptions<Config>();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
    }
}