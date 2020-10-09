using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;
using System.IO;
using CustomBatteries.API;
using InfinityBattery.Prefabs;
using UnityEngine;
using SMLHelper.V2.Utility;
#if SN1
using Sprite = Atlas.Sprite;
#endif

namespace InfinityBattery
{
    [QModCore]
    public static class Main
    {
        private static Assembly myAssembly = Assembly.GetExecutingAssembly();
        private static string ModPath = Path.GetDirectoryName(myAssembly.Location);
        private static string AssetsFolder = Path.Combine(ModPath, "Assets");

        public static Sprite Icon = ImageUtils.LoadSpriteFromFile(Path.Combine(Main.AssetsFolder, "icon.png"));
        public static Texture2D Skin => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "skin.png"));
        public static Texture2D Illum => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "skin_illum.png"));

        private static InfinityBatteryItem InfinityBatteryItem = new InfinityBatteryItem();
        public static CbItemPack InfinityPack { get; private set; }

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(myAssembly, $"MrPurple6411_{myAssembly.GetName().Name}");
            InfinityPack = InfinityBatteryItem.PatchAsBattery();

        }
    }
}