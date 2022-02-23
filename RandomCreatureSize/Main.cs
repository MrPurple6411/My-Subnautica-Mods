using BepInEx;

namespace RandomCreatureSize
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Handlers;
    using System.Reflection;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
         internal static Config SmcConfig { get; private set; } 
        internal static CreatureConfig CreatureConfig;

        public void Start()
        {
            SmcConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"Coticvo_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}