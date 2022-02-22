using BepInEx;

namespace SpecialtyManifold
{
    using HarmonyLib;
    using SMCLib.Handlers;
    using Configuration;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        internal static Config SmcConfig { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void  Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}