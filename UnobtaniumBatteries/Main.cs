#if SN1
namespace UnobtaniumBatteries
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System.Reflection;
    using System.IO;
    using CustomBatteries.API;
    using SMLHelper.V2.Handlers;
    using System.Collections.Generic;
    using System;
    using SMLHelper.V2.Utility;
    using UnityEngine;
    using UnobtaniumBatteries.MonoBehaviours;

#if SN1
    using Sprite = Atlas.Sprite;
#endif


    [QModCore]
    public static class Main
    {
        private static Assembly myAssembly = Assembly.GetExecutingAssembly();
        private static string ModPath = Path.GetDirectoryName(myAssembly.Location);
        internal static string AssetsFolder = Path.Combine(ModPath, "Assets");

        public static List<Type> typesToMakePickupable = new List<Type>() { typeof(ReaperLeviathan), typeof(GhostLeviathan), typeof(Warper) };
        public static List<TechType> unobtaniumBatteries = new List<TechType>();

        private static CbPowerCell _UnobtaniumCell = null;
        private static CbBattery _UnobtaniumBattery = null;

        public static CbBattery UnobtaniumBattery
        {
            get => _UnobtaniumBattery;
            private set
            {
                _UnobtaniumBattery = value;
                _UnobtaniumBattery.Patch();
            }
        }
        public static CbPowerCell UnobtaniumCell
        {
            get => _UnobtaniumCell;
            private set
            {
                _UnobtaniumCell = value;
                _UnobtaniumCell.Patch();
            }
        }


        [QModPatch]
        public static void Load()
        {
            CreateAndPatchPrefabs();
            SetupIngredientsInventorySettings();
            Harmony.CreateAndPatchAll(myAssembly, $"MrPurple6411_{myAssembly.GetName().Name}");
        }

        private static void SetupIngredientsInventorySettings()
        {
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
            if(reaper != null)
                SpriteHandler.RegisterSprite(TechType.ReaperLeviathan, reaper);

            Sprite ghost = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "ghost_icon.png"));
            if(ghost != null)
                SpriteHandler.RegisterSprite(TechType.GhostLeviathan, ghost);

            Sprite warper = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "warper_icon.png"));
            if(warper != null)
                SpriteHandler.RegisterSprite(TechType.Warper, warper);

            CraftDataHandler.SetItemSize(TechType.ReaperLeviathan, new Vector2int(5, 5));
            CraftDataHandler.SetItemSize(TechType.GhostLeviathan, new Vector2int(6, 6));
            CraftDataHandler.SetItemSize(TechType.Warper, new Vector2int(3, 3));
        }

        private static void CreateAndPatchPrefabs()
        {
            UnobtaniumBattery = new CbBattery()
            {
                ID = "UnobtaniumBattery",
                Name = "Unobtanium Battery",
                FlavorText = "Battery that constantly keeps 1 Million Power",
                EnergyCapacity = 1000000,
                CraftingMaterials = new List<TechType>() { TechType.ReaperLeviathan, TechType.GhostLeviathan, TechType.Warper },
                UnlocksWith = TechType.Warper,

                CustomIcon = ImageUtils.LoadSpriteFromFile(Path.Combine(Main.AssetsFolder, "battery_icon.png")),
                CBModelData = new CBModelData()
                {
                    CustomTexture = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_skin.png")),
                    CustomNormalMap = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_normal.png")),
                    CustomSpecMap = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_spec.png")),
                    CustomIllumMap = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "battery_illum.png")),
                    CustomIllumStrength = 1f,
                    UseIonModelsAsBase = true
                },

                EnhanceGameObject = new Action<GameObject>((o) => EnhanceGameObject(o))
            };
            unobtaniumBatteries.Add(Main.UnobtaniumBattery.TechType);

            UnobtaniumCell = new CbPowerCell()
            {
                EnergyCapacity = 1000000,
                ID = "UnobtaniumCell",
                Name = "Unobtanium Cell",
                FlavorText = "Power Cell that constantly keeps 1 Million Power",
                CraftingMaterials = new List<TechType>() { Main.UnobtaniumBattery.TechType },
                UnlocksWith = TechType.Warper,

                CustomIcon = ImageUtils.LoadSpriteFromFile(Path.Combine(Main.AssetsFolder, "cell_icon.png")),
                CBModelData = new CBModelData()
                {
                    CustomTexture = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "cell_skin.png")),
                    CustomNormalMap = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "cell_normal.png")),
                    CustomSpecMap = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "cell_spec.png")),
                    CustomIllumMap = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "cell_illum.png")),
                    CustomIllumStrength = 1f,
                    UseIonModelsAsBase = true
                },

                EnhanceGameObject = new Action<GameObject>((o) => EnhanceGameObject(o))
            };
            unobtaniumBatteries.Add(Main.UnobtaniumCell.TechType);
        }

        public static void EnhanceGameObject(GameObject gameObject)
        {
            gameObject.EnsureComponent<UnobtaniumBehaviour>();
        }
    }
}
#endif