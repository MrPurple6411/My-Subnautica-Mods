using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;
using System.IO;
using CustomBatteries.API;
using UnobtaniumBatteries.Prefabs;
using SMLHelper.V2.Handlers;
using System.Collections.Generic;
using System;
using SMLHelper.V2.Utility;

#if SN1
using Sprite = Atlas.Sprite;
#elif BZ
using UnityEngine;
#endif

namespace UnobtaniumBatteries
{
    [QModCore]
    public static class Main
    {
        private static Assembly myAssembly = Assembly.GetExecutingAssembly();
        private static string ModPath = Path.GetDirectoryName(myAssembly.Location);
        internal static string AssetsFolder = Path.Combine(ModPath, "Assets");

        public static List<Type> typesToMakePickupable = new List<Type>() { typeof(ReaperLeviathan), typeof(GhostLeviathan), typeof(Warper) };

        public static CbItemPack UnobtaniumCellPack { get; private set; }
        public static CbItemPack UnobtaniumBatteryPack { get; private set; }

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(myAssembly, $"MrPurple6411_{myAssembly.GetName().Name}");

            UnobtaniumBatteryPack = new UnobtaniumBatteryItem().PatchAsBattery();
            UnobtaniumCellPack = new UnobtaniumPowerCellItem().PatchAsPowerCell();

            WaterParkCreature.waterParkCreatureParameters[TechType.ReaperLeviathan] = new WaterParkCreatureParameters(0.01f, 0.06f, 1f, 3f, false);
            WaterParkCreature.waterParkCreatureParameters[TechType.GhostLeviathan] = new WaterParkCreatureParameters(0.01f, 0.06f, 1f, 3f, false);
            WaterParkCreature.waterParkCreatureParameters[TechType.Warper] = new WaterParkCreatureParameters(0.05f, 0.2f, 1f, 3f, false);

            LanguageHandler.SetTechTypeName(TechType.ReaperLeviathan, "Reaper Leviathan");
            LanguageHandler.SetTechTypeTooltip(TechType.ReaperLeviathan, "The Reaper Leviathan is an aggressive leviathan class species usually found swimming in large open areas.");

            LanguageHandler.SetTechTypeName(TechType.Warper, "Warper");
            LanguageHandler.SetTechTypeTooltip(TechType.Warper, "The Warper, or the Self-Warping Quarantine Enforcer Unit as named by the Precursors, is a bio-mechanical life form created by the Precursor race to hunt infected lifeforms.");

            LanguageHandler.SetTechTypeName(TechType.GhostLeviathan, "Ghost Leviathan");
            LanguageHandler.SetTechTypeTooltip(TechType.GhostLeviathan, "While the Ghost Leviathan is bigger then a Reaper Leviathan its aggression is territorial in nature, not predatory");

            Sprite reaper = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "reaper_icon.png"));
            if (reaper != null)
                SpriteHandler.RegisterSprite(TechType.ReaperLeviathan, reaper);

            Sprite ghost = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "ghost_icon.png"));
            if (ghost != null)
                SpriteHandler.RegisterSprite(TechType.GhostLeviathan, ghost);

            Sprite warper = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "warper_icon.png"));
            if (warper != null)
                SpriteHandler.RegisterSprite(TechType.Warper, warper);

            CraftDataHandler.SetItemSize(TechType.ReaperLeviathan, new Vector2int(5, 5));
            CraftDataHandler.SetItemSize(TechType.GhostLeviathan, new Vector2int(6, 6));
            CraftDataHandler.SetItemSize(TechType.Warper, new Vector2int( 3, 3));

        }
    }
}