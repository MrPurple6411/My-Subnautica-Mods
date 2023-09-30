namespace BaseKits.Prefabs;

using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static CraftData;

internal class CloneBasePiece : CustomPrefab
{
	[SetsRequiredMembers]
    internal CloneBasePiece(TechType typeToClone, TechType kitTechType): 
		base($"CBP_{typeToClone}", $"{Language.main.Get(typeToClone)}", "Built from a Kit!", SpriteManager.Get(typeToClone))
	{
		if (CraftData.GetBuilderIndex(typeToClone, out var group, out var category, out _))
			this.SetUnlock(kitTechType).WithPdaGroupCategoryAfter(group, category, typeToClone).SetBuildable();
		this.SetRecipe(new() { craftAmount = 1, Ingredients = new List<Ingredient>() { new(kitTechType, 1) } });
		CraftDataHandler.SetBackgroundType(Info.TechType, CraftData.BackgroundType.PlantAir);

		SetGameObject(new CloneTemplate(Info, typeToClone));
        Register();
    }
}