#if SUBNAUTICA
namespace MoreSeamothDepth;

using HarmonyLib;
using Nautilus.Handlers;
using System;
using System.Reflection;

using BepInEx;
using MoreSeamothDepth.Modules;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
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
#endif