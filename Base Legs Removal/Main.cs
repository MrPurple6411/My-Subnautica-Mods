using BepInEx;

namespace Base_Legs_Removal
{
    using Configuration;
    using HarmonyLib;
    using SMCLib.Handlers;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static Config SmcConfig { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void Awake()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{PluginInfo.PLUGIN_GUID}");
        }
    }
}