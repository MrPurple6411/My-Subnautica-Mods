using HarmonyLib;
using QModManager.API.ModLoading;
using InfinityBattery.Configuration;
using SMLHelper.V2.Handlers;
using System.Reflection;
using System.IO;
using CustomBatteries.API;
using InfinityBattery.Prefabs;

namespace InfinityBattery
{
    [QModCore]
    public static class Main
    {
        internal static Assembly myAssembly = Assembly.GetExecutingAssembly();
        internal static string ModPath = Path.GetDirectoryName(myAssembly.Location);
        internal static string AssetsFolder = Path.Combine(ModPath, "Assets");
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static InfinityBatteryItem InfinityBatteryItem { get; } = new InfinityBatteryItem();
        public static CbItemPack InfinityPack { get; private set; }

        [QModPatch]
        public static void Load()
        {
            //Harmony.CreateAndPatchAll(myAssembly, $"MrPurple6411_{myAssembly.GetName().Name}");
            InfinityPack = InfinityBatteryItem.PatchAsBattery();

        }
    }
}