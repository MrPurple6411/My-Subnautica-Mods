using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;
using System.IO;
using CustomBatteries.API;
using UnobtaniumBatteries.Prefabs;
using SMLHelper.V2.Handlers;

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

            LanguageHandler.SetTechTypeName(TechType.ReaperLeviathan, "Reaper Leviathan");
            LanguageHandler.SetTechTypeTooltip(TechType.ReaperLeviathan, "The Reaper Leviathan is an aggressive leviathan class fauna species usually found swimming in large open areas.");

            LanguageHandler.SetTechTypeName(TechType.Warper, "Warper");
            LanguageHandler.SetTechTypeTooltip(TechType.Warper, "The Warper, or the Self-Warping Quarantine Enforcer Unit as named by the Precursors, is a bio-mechanical life form created by the Precursor race to hunt infected lifeforms.");

            LanguageHandler.SetTechTypeName(TechType.GhostLeviathan, "Ghost Leviathan");
            LanguageHandler.SetTechTypeTooltip(TechType.GhostLeviathan, "While the Ghost Leviathan is bigger then a Reaper Leviathan its aggression is territorial in nature, not predatory");

        }
    }
}