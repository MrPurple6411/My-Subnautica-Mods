using BepInEx;

namespace ImprovedPowerNetwork
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Handlers;
    using System.Reflection;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

         internal static Config SmcConfig { get; private set; } 

        public void Start()
        {
            SmcConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}