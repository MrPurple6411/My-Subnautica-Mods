using BepInEx;
using BepInEx.Logging;

namespace PowerOrder
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Utility;
    using SMCLib.Handlers;
    using System.Reflection;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        internal static Config config = new();
        internal static uGUI_OptionsPanel optionsPanel;
        internal static ManualLogSource logSource;
        
        public void Start()
        {
            logSource = Logger;
            OptionsPanelHandler.RegisterModOptions(new Options());
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "subnautica.powerorder.mod");
            Logger.LogInfo("Patching complete.");
        }
    }
}