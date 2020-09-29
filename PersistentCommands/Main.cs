using HarmonyLib;
using QModManager.API.ModLoading;
using PersistentCommands.Configuration;
using SMLHelper.V2.Handlers;
using System.Reflection;

namespace PersistentCommands
{
    [QModCore]
    public static class Main
    {
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}