namespace BaseKits.Prefabs;

using Nautilus.Assets;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using static UWE.TUXOIL;
using Nautilus.Utility;

internal class KitFabricator: CustomPrefab
{
	public CraftTree.Type treeType;

	[SetsRequiredMembers]
    internal KitFabricator(string KitFab): base(KitFab, "Base Kit Fabricator", "Used to compress Base construction materials into a Construction Kit", SpriteManager.Get(TechType.Fabricator))
	{
		CraftDataHandler.SetBackgroundType(Info.TechType, CraftData.BackgroundType.PlantAir);

		if(CraftData.GetBuilderIndex(TechType.Workbench, out var group, out var category, out _))
			this.SetUnlock(TechType.Workbench).WithPdaGroupCategoryAfter(group, category, TechType.Workbench);

		this.SetRecipe(CraftDataHandler.GetRecipeData(TechType.Fabricator));

		var gadget = this.CreateFabricator(out treeType);
		gadget.AddTabNode(Main.RoomsMenu, "Rooms", SpriteManager.Get(TechType.BaseRoom));
		gadget.AddTabNode(Main.CorridorMenu, "Corridors", SpriteManager.Get(TechType.BaseCorridorX));
		gadget.AddTabNode(Main.ModuleMenu, "Modules", SpriteManager.Get(TechType.BaseBioReactor));
		gadget.AddTabNode(Main.UtilityMenu, "Utilities", SpriteManager.Get(TechType.BaseHatch));

		FabricatorTemplate fabPrefab = new FabricatorTemplate(Info, treeType)
		{
			FabricatorModel = FabricatorTemplate.Model.Fabricator,
			ColorTint = UnityEngine.Color.magenta,
			ConstructableFlags = ConstructableFlags.Wall|ConstructableFlags.Base|ConstructableFlags.Submarine|ConstructableFlags.Inside
		};

		SetGameObject(fabPrefab);

		Register();
    }
}
