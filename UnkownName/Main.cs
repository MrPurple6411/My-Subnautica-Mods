using BepInEx;

namespace UnKnownName
{
    using System.Reflection;
    using HarmonyLib;
    using SMCLib.Handlers;
    using Configuration;


    [BepInPlugin(UnknownName.PluginInfo.PLUGIN_GUID, UnknownName.PluginInfo.PLUGIN_NAME, UnknownName.PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
         internal static Config SmcConfig { get; private set; } 

        public void Start()
        {
            SmcConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}