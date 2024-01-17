namespace SpecialtyManifold;

using HarmonyLib;using BepInEx;
using SpecialtyManifold.Patches;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
public class Main: BaseUnityPlugin
{
	public static ManualLogSource Log { get; private set; }

    private void Awake()
    {
		Log = Logger;
        Harmony.CreateAndPatchAll(typeof(PlayerPatcher), MyPluginInfo.PLUGIN_GUID);
    }
}