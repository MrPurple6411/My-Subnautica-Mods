using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;
using System.IO;
using CustomBatteries.API;
using InfinityPowerCell.Prefabs;
using UnityEngine;
using SMLHelper.V2.Utility;
#if SN1
using Sprite = Atlas.Sprite;
#endif

namespace InfinityPowerCell
{
    [QModCore]
    public static class Main
    {
        private static Assembly myAssembly = Assembly.GetExecutingAssembly();
        private static string ModPath = Path.GetDirectoryName(myAssembly.Location);
        private static string AssetsFolder = Path.Combine(ModPath, "Assets");

        public static Sprite Icon = ImageUtils.LoadSpriteFromFile(Path.Combine(Main.AssetsFolder, "icon.png"));

        public static Texture2D Texture => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "skin.png"));

        public static Texture2D Illum => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "skin_illum.png"));

        public static Texture2D Normal => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "skin_normal.png"));

        public static Texture2D Spec => ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "skin_spec.png"));

        private static InfinityPowerCellItem InfinityPowerCellItem = new InfinityPowerCellItem();

        public static CbItemPack InfinityCellPack { get; private set; }

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(myAssembly, $"MrPurple6411_{myAssembly.GetName().Name}");
            InfinityCellPack = InfinityPowerCellItem.PatchAsPowerCell();
        }
    }
}