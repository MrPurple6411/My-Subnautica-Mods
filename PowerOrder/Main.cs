namespace PowerOrder;

using HarmonyLib;
using Configuration;
using Nautilus.Handlers;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
public class Main: BaseUnityPlugin
{
    internal static SMLConfig SMLConfig = new();
    internal static new ManualLogSource Logger;

    private void Awake()
    {
		Logger = base.Logger;
        OptionsPanelHandler.RegisterModOptions(new Options());
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
        Logger.LogInfo("Patching complete.");
    }
}