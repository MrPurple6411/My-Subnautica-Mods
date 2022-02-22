using BepInEx;

namespace UnKnownName
{
    using System.Reflection;
    using HarmonyLib;
    using SMCLib.Handlers;
    using Configuration;


    public class Main:BaseUnityPlugin
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void  Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}