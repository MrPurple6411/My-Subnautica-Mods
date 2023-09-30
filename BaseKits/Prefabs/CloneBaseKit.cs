namespace BaseKits.Prefabs;

using Nautilus.Assets;
using Nautilus.Handlers;
using System.Collections.Generic;
using Nautilus.Crafting;
using Nautilus.Assets.Gadgets;
using System.Diagnostics.CodeAnalysis;

public class CloneBaseKit : CustomPrefab
{
    private static readonly List<TechType> _unlockRequired = new()
    {
        TechType.BaseBulkhead, TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool,
        TechType.BaseBioReactor, TechType.BaseNuclearReactor, TechType.BaseObservatory, TechType.BaseUpgradeConsole,
        TechType.BaseFiltrationMachine, TechType.BaseWaterPark, TechType.BaseLargeRoom, TechType.BaseLargeGlassDome, 
        TechType.BaseGlassDome, TechType.BaseControlRoom, TechType.BasePartition, TechType.BasePartitionDoor
    };

	[SetsRequiredMembers]
    internal CloneBaseKit(TechType typeToClone, string FabricatorMenu, CraftTree.Type PurpleKitFabricator): 
		base($"Kit_{typeToClone}", $"{Language.main.Get(typeToClone)} Kit", "Super Compressed Base in a Kit", SpriteManager.Get(typeToClone))
    {

		SetGameObject(() => Utils.CreateGenericLoot(Info.TechType));

		this.SetRecipe(CraftDataHandler.GetRecipeData(typeToClone) ?? new RecipeData() { craftAmount = 0 })
			.WithFabricatorType(PurpleKitFabricator)
			.WithStepsToFabricatorTab(new[] { FabricatorMenu })
			.WithCraftingTime(10f);

		var scanningGadget = this.SetUnlock(typeToClone);

		if (CraftData.GetBuilderIndex(typeToClone, out var originalGroup, out var originalCategory, out _))
        {
			var originalCategoryString = Language.main.Get(uGUI_BlueprintsTab.techCategoryStrings.Get(originalCategory));
			var tgs = $"{originalGroup}_Kits"; 


			if (!EnumHandler.TryGetValue(tgs, out TechGroup group))
            {
                group = EnumHandler.AddEntry<TechGroup>(tgs).WithPdaInfo($"{originalGroup} - Kits");
            }

			var tcs = $"{originalCategory}_Kits";
            if(!EnumHandler.TryGetValue(tcs, out TechCategory category))
            {
                category = EnumHandler.AddEntry<TechCategory>(tcs).WithPdaInfo($"{originalCategoryString} - Kits").RegisterToTechGroup(group);
			}

			scanningGadget.WithPdaGroupCategory(group, category);
		}

        CraftDataHandler.SetBackgroundType(Info.TechType, CraftData.BackgroundType.PlantAir);
        Register();
    }
}