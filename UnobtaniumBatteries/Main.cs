namespace UnobtaniumBatteries
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System.Reflection;
    using System.IO;
    using CustomBatteries.API;
    using System.Collections.Generic;
    using SMLHelper.V2.Utility;
    using UnityEngine;
    using MonoBehaviours;

#if SUBNAUTICA_STABLE
    using SMLHelper.V2.Handlers;
#endif


    [QModCore]
    public static class Main
    {
        private static readonly Assembly myAssembly = Assembly.GetExecutingAssembly();
        private static readonly string ModPath = Path.GetDirectoryName(myAssembly.Location);
        private static readonly string AssetsFolder = Path.Combine(ModPath, "Assets");
        public static readonly List<TechType> unobtaniumBatteries = new();
#if SN1
        public static readonly List<TechType> typesToMakePickupable = new() { TechType.ReaperLeviathan, TechType.GhostLeviathan, TechType.Warper };
#endif
        
        [QModPatch]
        public static void Load()
        {
            CreateAndPatchPrefabs();
#if SN1
            SetupIngredientsInventorySettings();
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

            var reaper = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "reaper_icon.png"));
            if(reaper != null)
                SpriteHandler.RegisterSprite(TechType.ReaperLeviathan, reaper);

            var ghost = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "ghost_icon.png"));
            if(ghost != null)
                SpriteHandler.RegisterSprite(TechType.GhostLeviathan, ghost);

            var warper = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "warper_icon.png"));
            if(warper != null)
                SpriteHandler.RegisterSprite(TechType.Warper, warper);

            CraftDataHandler.SetItemSize(TechType.ReaperLeviathan, new Vector2int(5, 5));
            CraftDataHandler.SetItemSize(TechType.GhostLeviathan, new Vector2int(6, 6));
            CraftDataHandler.SetItemSize(TechType.Warper, new Vector2int(3, 3));
#endif
            Harmony.CreateAndPatchAll(myAssembly, $"MrPurple6411_{myAssembly.GetName().Name}");
        }

        private static void CreateAndPatchPrefabs()
        {
            var UnobtaniumBattery = new CbBattery()
            {
                ID = "UnobtaniumBattery",
                Name = "Unobtanium Battery",
                FlavorText = "Battery that constantly keeps 1 Million Power",
                EnergyCapacity = 1000000,
#if SN1
                CraftingMaterials = new List<TechType>() { TechType.ReaperLeviathan, TechType.GhostLeviathan, TechType.Warper },
                UnlocksWith = TechType.Warper,
#elif BZ
                CraftingMaterials = new List<TechType>() { TechType.SquidShark, TechType.Jellyfish, TechType.LilyPaddler },
                UnlocksWith = TechType.TeleportationTool,
#endif

                CustomIcon = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "battery_icon.png")),
                CBModelData = new CBModelData()
                {
                    CustomTexture = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "battery_skin.png")),
                    CustomNormalMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "battery_normal.png")),
                    CustomSpecMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "battery_spec.png")),
                    CustomIllumMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "battery_illum.png")),
                    CustomIllumStrength = 1f,
                    UseIonModelsAsBase = true
                },

                EnhanceGameObject = EnhanceGameObject
            };
            UnobtaniumBattery.Patch();
            unobtaniumBatteries.Add(UnobtaniumBattery.TechType);

            var UnobtaniumCell = new CbPowerCell()
            {
                EnergyCapacity = 1000000,
                ID = "UnobtaniumCell",
                Name = "Unobtanium Cell",
                FlavorText = "Power Cell that constantly keeps 1 Million Power",
                CraftingMaterials = new List<TechType>() { UnobtaniumBattery.TechType },
                UnlocksWith = UnobtaniumBattery.TechType,

                CustomIcon = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "cell_icon.png")),
                CBModelData = new CBModelData()
                {
                    CustomTexture = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "cell_skin.png")),
                    CustomNormalMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "cell_normal.png")),
                    CustomSpecMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "cell_spec.png")),
                    CustomIllumMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "cell_illum.png")),
                    CustomIllumStrength = 1f,
                    UseIonModelsAsBase = true
                },

                EnhanceGameObject = EnhanceGameObject
            };
            UnobtaniumCell.Patch();
            unobtaniumBatteries.Add(UnobtaniumCell.TechType);
        }

        private static void EnhanceGameObject(GameObject gameObject)
        {
            gameObject.EnsureComponent<UnobtaniumBehaviour>();
        }
    }
}