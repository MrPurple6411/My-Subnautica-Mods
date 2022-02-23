using BepInEx;

namespace Increased_Resource_Spawns
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Handlers;
    using System.Linq;
    using System.Reflection;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
         internal static Config SmcConfig { get; private set; } 

        public void Start()
        {
            SmcConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            SmcConfig.Blacklist = SmcConfig.Blacklist.Distinct().ToList();
            SmcConfig.WhiteList = SmcConfig.WhiteList.Distinct().ToList();
            SmcConfig.Save();


            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}