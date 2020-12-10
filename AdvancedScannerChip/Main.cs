using AdvancedScannerChip.Configuration;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using System.Reflection;

namespace AdvancedScannerChip
{
    [QModCore]
    public static class Main
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();
        //internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}