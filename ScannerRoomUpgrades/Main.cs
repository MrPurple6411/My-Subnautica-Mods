using HarmonyLib;
using QModManager.API.ModLoading;
using ScannerRoomUpgrades.Configuration;
using SMLHelper.V2.Handlers;
using System.Reflection;

namespace ScannerRoomUpgrades
{
    [QModCore]
    public static class Main
    {
        internal static readonly Config config = new Config();

        [QModPatch]
        public static void Load()
        {
            OptionsPanelHandler.RegisterModOptions(new Options());

            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}