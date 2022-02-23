using BepInEx;

namespace Base_Legs_Removal
{
    using Configuration;
    using HarmonyLib;
    using SMCLib.Handlers;
    using System.Reflection;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static Config SmcConfig { get; private set; }

        public void Start()
        {
            SmcConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{PluginInfo.PLUGIN_GUID}");
        }
    }
}