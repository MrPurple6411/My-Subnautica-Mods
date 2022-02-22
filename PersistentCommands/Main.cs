using BepInEx;

namespace PersistentCommands
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Handlers;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        public void  Awake()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}