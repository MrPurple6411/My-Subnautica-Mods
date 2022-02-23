using BepInEx;

#if SN1
namespace MoreSeamothDepth
{
    using HarmonyLib;
    using SMCLib.Handlers;
    using System;
    using System.Reflection;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        internal static Modules.SeamothHullModule4 moduleMK4 = new();
        internal static Modules.SeamothHullModule5 moduleMK5 = new();

        public void Start()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

                LanguageHandler.SetLanguageLine("Tooltip_VehicleHullModule3", "Enhances diving depth. Does not stack"); // To update conflicts about the maximity.

                moduleMK4.Patch();
                moduleMK5.Patch();

                Console.WriteLine("[MoreSeamothDepth] Succesfully patched!");
            }

            catch(Exception e)
            {
                Console.WriteLine("[MoreSeamothDepth] Caught exception! " + e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
#endif