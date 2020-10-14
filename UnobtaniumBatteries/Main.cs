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

        public static CbItemPack UnobtaniumCellPack { get; private set; }
        public static CbItemPack UnobtaniumBatteryPack { get; private set; }

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(myAssembly, $"MrPurple6411_{myAssembly.GetName().Name}");

            UnobtaniumBatteryPack = new UnobtaniumBatteryItem().PatchAsBattery();
            UnobtaniumCellPack = new UnobtaniumPowerCellItem().PatchAsPowerCell();

        }
    }
}