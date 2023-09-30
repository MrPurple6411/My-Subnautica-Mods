namespace SeamothThermal.Modules;

using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static CraftData;

public class SeamothThermalModule: CustomPrefab
{
	[SetsRequiredMembers]
    public SeamothThermalModule() : base(
        "SeamothThermalModule",
        "Seamoth thermal reactor",
        "Recharges power cells in hot areas. Doesn't stack.",
		SpriteManager.Get(TechType.ExosuitThermalReactorModule))
    {

		this.SetRecipe(new()
		{
			craftAmount = 1,
			Ingredients = new List<Ingredient>()
			{
				new(TechType.Kyanite, 1),
				new(TechType.Polyaniline, 2),
				new(TechType.WiringKit, 1)
			}
		}).WithFabricatorType(CraftTree.Type.SeamothUpgrades).WithStepsToFabricatorTab("SeamothModules");

		this.SetEquipment(EquipmentType.SeamothModule).WithQuickSlotType(QuickSlotType.Passive);

		if (GetBuilderIndex(TechType.SeamothSolarCharge, out var group, out var category, out _))
			this.SetUnlock(TechType.BaseUpgradeConsole).WithPdaGroupCategoryAfter(group, category, TechType.SeamothSolarCharge);

		SetGameObject(GetGameObjectAsync);

		Register();
	}

    public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {
        var taskResult = new TaskResult<GameObject>();
        yield return CraftData.InstantiateFromPrefabAsync(TechType.SeamothElectricalDefense, taskResult);
        var obj = taskResult.Get();
        
        // Get the TechTags and PrefabIdentifiers
        var techTag = obj.GetComponent<TechTag>();
        var prefabIdentifier = obj.GetComponent<PrefabIdentifier>();

        // Change them so they fit to our requirements.
        techTag.type = Info.TechType;
        prefabIdentifier.ClassId = Info.ClassID;
        gameObject.Set(obj);
    }
}
