namespace UnobtaniumBatteries;

using HarmonyLib;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Nautilus.Utility;
using UnityEngine;
using MonoBehaviours;using BepInEx;
using CustomBatteries.API;
using Nautilus.Handlers;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[BepInDependency(CustomBatteries.MyPluginInfo.PLUGIN_GUID, CustomBatteries.MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(CustomCommands.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
#if SUBNAUTICA
[BepInDependency(WarpersNoWarping.MyPluginInfo.PLUGIN_GUID, WarpersNoWarping.MyPluginInfo.PLUGIN_VERSION)]
#endif
public class Main : BaseUnityPlugin
{
	private static readonly Assembly myAssembly = Assembly.GetExecutingAssembly();
	private static readonly string ModPath = Path.GetDirectoryName(myAssembly.Location);
	private static readonly string AssetsFolder = Path.Combine(ModPath, "Assets");
	public static readonly List<TechType> unobtaniumBatteries = new();
	public static readonly List<TechType> typesToMakePickupable = new() 
#if SUBNAUTICA
	{ TechType.ReaperLeviathan, TechType.GhostLeviathan, TechType.Warper };
#elif BELOWZERO
	{ TechType.SquidShark, TechType.Jellyfish, TechType.LilyPaddler };
#endif

	private void Awake()
	{
		CreateAndPatchPrefabs();
		SetupIngredientsInventorySettings();
	}

	private static void SetupIngredientsInventorySettings()
	{
#if SUBNAUTICA
		LanguageHandler.SetTechTypeName(TechType.ReaperLeviathan, "Reaper Leviathan");
		LanguageHandler.SetTechTypeTooltip(TechType.ReaperLeviathan, "The Reaper Leviathan is an aggressive leviathan class species usually found swimming in large open areas.");

		LanguageHandler.SetTechTypeName(TechType.Warper, "Warper");
		LanguageHandler.SetTechTypeTooltip(TechType.Warper, "The Warper, or the Self-Warping Quarantine Enforcer Unit as named by the Precursors, is a bio-mechanical life form created by the Precursor race to hunt infected lifeforms.");

		LanguageHandler.SetTechTypeName(TechType.GhostLeviathan, "Ghost Leviathan");
		LanguageHandler.SetTechTypeTooltip(TechType.GhostLeviathan, "While the Ghost Leviathan is bigger then a Reaper Leviathan its aggression is territorial in nature, not predatory");


		var filePathToReaper = Path.Combine(AssetsFolder, "reaper_icon.png");

		if (File.Exists(filePathToReaper))
		{
			var reaper = ImageUtils.LoadSpriteFromFile(filePathToReaper);
			if (reaper != null)
				SpriteHandler.RegisterSprite(TechType.ReaperLeviathan, reaper);
		}

		var filePathToGhost = Path.Combine(AssetsFolder, "ghost_icon.png");

		if (File.Exists(filePathToGhost))
		{
			var ghost = ImageUtils.LoadSpriteFromFile(filePathToGhost);
			if (ghost != null)
				SpriteHandler.RegisterSprite(TechType.GhostLeviathan, ghost);
		}

		var filePathToWarper = Path.Combine(AssetsFolder, "warper_icon.png");

		if (File.Exists(filePathToWarper))
		{
			var warper = ImageUtils.LoadSpriteFromFile(filePathToWarper);
			if (warper != null)
				SpriteHandler.RegisterSprite(TechType.Warper, warper);
		}

		CraftDataHandler.SetItemSize(TechType.ReaperLeviathan, new Vector2int(5, 5));
		CraftDataHandler.SetItemSize(TechType.GhostLeviathan, new Vector2int(6, 6));
		CraftDataHandler.SetItemSize(TechType.Warper, new Vector2int(3, 3));
#elif BELOWZERO
		
		

#endif

		Harmony.CreateAndPatchAll(myAssembly, MyPluginInfo.PLUGIN_GUID);
	}

	private static void CreateAndPatchPrefabs()
	{
		var UnobtaniumBattery = new CbBattery()
		{
			ID = "UnobtaniumBattery",
			Name = "Unobtanium Battery",
			FlavorText = "Battery that constantly keeps 1 Million Power",
			EnergyCapacity = 1000000,
			CraftingMaterials = typesToMakePickupable,
			UnlocksWith =
#if SUBNAUTICA
			TechType.Warper,
#elif BELOWZERO
			TechType.SquidShark,
#endif

			CustomIcon = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "battery_icon.png")),
			CBModelData = new CBModelData()
			{
				CustomTexture = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "battery_skin.png")),
				CustomNormalMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "battery_normal.png")),
				CustomSpecMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "battery_spec.png")),
				CustomIllumMap = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "battery_illum.png")),
				CustomIllumStrength = 1f,
				UseIonModelsAsBase = true,
			},

			EnhanceGameObject = EnhanceGameObject
		};
		UnobtaniumBattery.Patch();
		unobtaniumBatteries.Add(UnobtaniumBattery.TechType);

		var skinPath = Path.Combine(AssetsFolder, "cell_skin.png");
		var normalPath = Path.Combine(AssetsFolder, "cell_normal.png");
		var specPath = Path.Combine(AssetsFolder, "cell_spec.png");
		var illumPath = Path.Combine(AssetsFolder, "cell_illum.png");

		var skin = ImageUtils.LoadTextureFromFile(skinPath);
		var normal = ImageUtils.LoadTextureFromFile(normalPath);
		var spec = ImageUtils.LoadTextureFromFile(specPath);
		var illum = ImageUtils.LoadTextureFromFile(illumPath);

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
				CustomTexture = skin,
				CustomNormalMap = normal,
				CustomSpecMap = spec,
				CustomIllumMap = illum,
				CustomIllumStrength = 1f,
				UseIonModelsAsBase = true,
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