namespace PersistentCommands;

using HarmonyLib;
using Configuration;
using Nautilus.Handlers;using BepInEx;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
public class Main: BaseUnityPlugin
{
	public static ManualLogSource Log { get; internal set; }
	internal static SMLConfig Config { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();

    private void Awake()
    {
		Log = Logger;
        Harmony.CreateAndPatchAll(typeof(Patches.Patches), MyPluginInfo.PLUGIN_GUID);
    }
}