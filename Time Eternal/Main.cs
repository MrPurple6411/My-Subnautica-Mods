namespace Time_Eternal
{
    using HarmonyLib;
    using SMCLib.Handlers;
    using System.Reflection;
    using Configuration;
    using BepInEx;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
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