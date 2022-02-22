using BepInEx;
using BepInEx.Logging;

namespace BuildingTweaks
{
    using Configuration;
    using HarmonyLib;
    using SMCLib.Handlers;
    using System.Reflection;

    public  class Main: BaseUnityPlugin
    {
        public static Config SmcConfig { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static ManualLogSource logSource;

        public  void Awake()
        {
            logSource = Logger;
            var assembly = Assembly.GetExecutingAssembly();
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}