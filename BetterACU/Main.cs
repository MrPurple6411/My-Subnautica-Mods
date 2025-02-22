namespace BetterACU;

using HarmonyLib;
using System.Reflection;
using BepInEx;
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
	internal new static ManualLogSource Logger;

    private void Awake()
    {
		Logger = base.Logger;
		Configuration.Config.Register();
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
		Logger.LogInfo($"Loaded {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION}");
	}
}