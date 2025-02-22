#if BELOWZERO
namespace ExtravagantGifts;

using HarmonyLib;
using BepInEx;
using System.Reflection;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[BepInProcess("SubnauticaZero.exe")]
public class Main : BaseUnityPlugin
{
	private void Awake()
	{
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
	}
}
#endif