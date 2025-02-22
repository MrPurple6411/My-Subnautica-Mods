namespace MoreSeamothDepth;

using HarmonyLib;
using Nautilus.Handlers;
using System;
using System.Reflection;

using BepInEx;
using MoreSeamothDepth.Modules;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main : BaseUnityPlugin
{
	internal static TechType moduleMK4;
	internal static TechType moduleMK5;

	private void Awake()
	{
		try
		{
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
			LanguageHandler.SetLanguageLine("Tooltip_VehicleHullModule3", "Enhances diving depth. Does not stack"); // To update conflicts about the maximity.
			CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "SeamothDepthModules", "Seamoth Depth Upgrades", SpriteManager.Get(TechType.VehicleHullModule1));
			CraftTreeHandler.RemoveNode(CraftTree.Type.Workbench, "VanillaWorkbench", "VehicleHullModule2");
			CraftTreeHandler.RemoveNode(CraftTree.Type.Workbench, "VanillaWorkbench", "VehicleHullModule3");

#if BELOWZERO
			CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, TechType.VehicleHullModule1, "SeamothDepthModules");
#endif
			CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, TechType.VehicleHullModule2, "SeamothDepthModules");
			CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, TechType.VehicleHullModule3, "SeamothDepthModules");

			moduleMK4 = new SeamothHullModule4().Info.TechType;
			moduleMK5 = new SeamothHullModule5().Info.TechType;
			Logger.LogInfo("Succesfully patched!");
		}
		catch (Exception e)
		{
			Logger.LogError(e.InnerException.Message);
			Logger.LogError(e.InnerException.StackTrace);
		}
	}
}