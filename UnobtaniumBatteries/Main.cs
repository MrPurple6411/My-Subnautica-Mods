using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;
using System.IO;
using CustomBatteries.API;
using UnobtaniumBatteries.Prefabs;

namespace UnobtaniumBatteries
{
    [QModCore]
    public static class Main
    {
        private static Assembly myAssembly = Assembly.GetExecutingAssembly();
        private static string ModPath = Path.GetDirectoryName(myAssembly.Location);
        internal static string AssetsFolder = Path.Combine(ModPath, "Assets");

        public static CbItemPack InfinityCellPack { get; } = new UnobtaniumPowerCellItem().PatchAsPowerCell();

        public static CbItemPack InfinityBatteryPack { get; } = new UnobtaniumBatteryItem().PatchAsBattery();

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(myAssembly, $"MrPurple6411_{myAssembly.GetName().Name}");
        }
    }
}