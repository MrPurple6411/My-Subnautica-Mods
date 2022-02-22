using BepInEx;

namespace ImprovedPowerNetwork
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Handlers;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        internal static Config SmcConfig { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void  Awake()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}