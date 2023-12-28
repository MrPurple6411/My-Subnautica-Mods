namespace BetterACU;

using Configuration;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;
using BepInEx;
using Nautilus.Utility;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
public class Main: BaseUnityPlugin
{
	internal static ManualLogSource Logger;

    private void Awake()
    {
		Logger = base.Logger;
		Configuration.Config.Register();
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
    }
}