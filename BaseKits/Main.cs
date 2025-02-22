namespace BaseKits;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;
using BepInEx;
using BepInEx.Logging;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using System;
using Nautilus.Utility;
using static CraftData;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main: BaseUnityPlugin
{

    private static readonly List<TechType> _roomsToClone = new()
    {
        TechType.BaseFoundation, TechType.BaseMapRoom, TechType.BaseMoonpool,
        TechType.BaseObservatory, TechType.BaseRoom
        ,TechType.BaseLargeRoom,TechType.BaseGlassDome,TechType.BaseLargeGlassDome, TechType.BasePartitionDoor,
        TechType.BasePartition
#if BELOWZERO
        , TechType.BaseControlRoom
#endif
    };

    private static readonly List<TechType> _corridorsToClone = new()
    {
        TechType.BaseCorridorGlassI, TechType.BaseCorridorGlassL,
        TechType.BaseCorridorI, TechType.BaseCorridorL, TechType.BaseCorridorT, TechType.BaseCorridorX

    };

    private static readonly List<TechType> _modulesToClone = new()
    {
        TechType.BaseBioReactor, TechType.BaseFiltrationMachine,
        TechType.BaseNuclearReactor, TechType.BaseUpgradeConsole, TechType.BaseWaterPark
    };

    private static readonly List<TechType> _utilitiesToClone = new()
    {
        TechType.BaseConnector, TechType.BaseBulkhead, TechType.BaseHatch,
        TechType.BaseLadder, TechType.BaseReinforcement, TechType.BaseWindow
        
    };

    internal const string LangFormat = "{0}Menu_{1}";
    internal const string SpriteFormat = "{0}_{1}";
    internal const string KitFab = "PurpleKitFabricator";
    internal const string RoomsMenu = "RoomsMenu";
    internal const string CorridorMenu = "CorridorMenu";
    internal const string ModuleMenu = "ModuleMenu";
    internal const string UtilityMenu = "UtilityMenu";

    internal static ManualLogSource logSource;

    private void Awake()
    {
        logSource = Logger;
        CoroutineHost.StartCoroutine(RegisterKits());
    }

	public CraftTree.Type purpleKitFabricator;

    private IEnumerator RegisterKits()
	{
		if (Language.main is null)
			yield return new WaitWhile(() => Language.main is null);
		
		CreateKitFabricator();
		ProcessTypes(_roomsToClone, RoomsMenu);
		ProcessTypes(_corridorsToClone, CorridorMenu);
		ProcessTypes(_modulesToClone, ModuleMenu);
		ProcessTypes(_utilitiesToClone, UtilityMenu);
	}

	private void CreateKitFabricator()
	{
		var fabricatorPrefab = new CustomPrefab(KitFab, "Base Kit Fabricator", "Used to compress Base construction materials into a Construction Kit", SpriteManager.Get(TechType.Fabricator));

		CraftDataHandler.SetBackgroundType(fabricatorPrefab.Info.TechType, CraftData.BackgroundType.PlantAir);

		if (CraftData.GetBuilderIndex(TechType.Workbench, out var group, out var category, out _))
			fabricatorPrefab.SetUnlock(TechType.Workbench).WithPdaGroupCategoryAfter(group, category, TechType.Workbench).SetBuildable().WithAnalysisTech(null, null, null);

		CraftDataHandler.SetRecipeData(fabricatorPrefab.Info.TechType, CraftDataHandler.GetRecipeData(TechType.Fabricator));

		var gadget = fabricatorPrefab.CreateFabricator(out purpleKitFabricator);
		gadget.AddTabNode(Main.RoomsMenu, "Rooms", SpriteManager.Get(TechType.BaseRoom));
		gadget.AddTabNode(Main.CorridorMenu, "Corridors", SpriteManager.Get(TechType.BaseCorridorX));
		gadget.AddTabNode(Main.ModuleMenu, "Modules", SpriteManager.Get(TechType.BaseBioReactor));
		gadget.AddTabNode(Main.UtilityMenu, "Utilities", SpriteManager.Get(TechType.BaseHatch));

		var fabPrefab = new FabricatorTemplate(fabricatorPrefab.Info, purpleKitFabricator)
		{
			FabricatorModel = FabricatorTemplate.Model.Fabricator,
			ColorTint = UnityEngine.Color.magenta,
			ConstructableFlags = ConstructableFlags.Wall | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.Inside,
			ModifyPrefab = (go) => go.SetActive(false)
		};

		fabricatorPrefab.SetGameObject(fabPrefab);

		fabricatorPrefab.Register();
	}

	private void ProcessTypes(List<TechType> typesToClone, string FabricatorMenu)
    {
        foreach(var techType in typesToClone)
		{
			CreateKitPrefab(FabricatorMenu, techType, out var kitTechType);
			CreatePeicePrefab(techType, kitTechType);
		}
	}

	private static void CreatePeicePrefab(TechType techType, TechType kitTechType)
	{


		var icon = SpriteManager.Get(techType);
		var cloneBasePiece = new CustomPrefab($"CBP_{techType}", $"{Language.main.Get(techType)}", "Built from a Kit!", icon);

		KnownTechHandler.SetAnalysisTechEntry(techType, new[] { cloneBasePiece.Info.TechType });
		if (GetBuilderIndex(techType, out var group, out var category, out _))
			cloneBasePiece.SetUnlock(kitTechType).WithPdaGroupCategoryAfter(group, category, techType).SetBuildable().WithAnalysisTech(null, null, null);

		CraftDataHandler.SetRecipeData(cloneBasePiece.Info.TechType, new() { craftAmount = 1, Ingredients = new List<Ingredient>() { new(kitTechType, 1) } });
		CraftDataHandler.SetBackgroundType(cloneBasePiece.Info.TechType, CraftData.BackgroundType.PlantAir);

		cloneBasePiece.SetGameObject(new CloneTemplate(cloneBasePiece.Info, techType) { ModifyPrefab = (go) => go.SetActive(false) });
		cloneBasePiece.Register();
	}

	private void CreateKitPrefab(string FabricatorMenu, TechType techType, out TechType kitTechType)
	{
		var kitPrefab = new CustomPrefab($"Kit_{techType}", $"{Language.main.Get(techType)} Kit", "Super Compressed Base in a Kit", SpriteManager.Get(techType));
		kitPrefab.SetGameObject(() => { var obj = global::Utils.CreateGenericLoot(kitPrefab.Info.TechType); obj.SetActive(false); return obj; });
		kitPrefab.SetRecipe(CraftDataHandler.GetRecipeData(techType) ?? new RecipeData() { craftAmount = 0 })
			.WithFabricatorType(purpleKitFabricator)
			.WithStepsToFabricatorTab(new[] { FabricatorMenu })
			.WithCraftingTime(10f);

		KnownTechHandler.SetAnalysisTechEntry(techType, new[] { kitPrefab.Info.TechType });
		var scanningGadget = kitPrefab.SetUnlock(techType).WithAnalysisTech(null, null, null);

		if (GetBuilderIndex(techType, out var originalGroup, out var originalCategory, out _))
		{
			var originalCategoryString = Language.main.Get(uGUI_BlueprintsTab.techCategoryStrings.Get(originalCategory));
			if(!_cache.TryGetValue(originalCategoryString, out var tuple))
				CreateGroupCategoryCache(originalGroup, originalCategory, originalCategoryString, out tuple);
			scanningGadget.WithPdaGroupCategory(tuple.Item1, tuple.Item2);
		}

		CraftDataHandler.SetBackgroundType(kitPrefab.Info.TechType, CraftData.BackgroundType.PlantAir);
		kitPrefab.Register();
		kitTechType = kitPrefab.Info.TechType;
	}

	private readonly Dictionary<string, Tuple<TechGroup, TechCategory>> _cache = new();

	private void CreateGroupCategoryCache(TechGroup originalGroup, TechCategory originalCategory, string originalCategoryString, out Tuple<TechGroup, TechCategory> tuple)
	{
		var tgs = $"{originalGroup}_Kits";
		var tcs = $"{originalCategory}_Kits";

		if (!EnumHandler.TryGetValue(tgs, out TechGroup group))
			group = EnumHandler.AddEntry<TechGroup>(tgs).WithPdaInfo($"{originalGroup} - Kits");

		if (!EnumHandler.TryGetValue(tcs, out TechCategory category))
			category = EnumHandler.AddEntry<TechCategory>(tcs).WithPdaInfo($"{originalCategoryString} - Kits").RegisterToTechGroup(group);

		tuple = new(group, category);
		_cache.Add(originalCategoryString, tuple);
	}
}