#if SUBNAUTICA
namespace MoreSeamothDepth.Modules;
using System.Collections;

using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets.Gadgets;
using static CraftData;

public class SeamothHullModule4 : CustomPrefab
{
	[SetsRequiredMembers]
	public SeamothHullModule4() : base("SeamothHullModule4", "Seamoth depth module MK4", "Enhances diving depth. Does not stack.", SpriteManager.Get(TechType.VehicleHullModule3))
	{
		this.SetRecipe(new()
		{
			Ingredients = new List<Ingredient>()
				{
					new(TechType.VehicleHullModule3, 1),
					new(TechType.PlasteelIngot, 1),
					new(TechType.Nickel, 2),
					new(TechType.AluminumOxide, 3)
				},
			craftAmount = 1
		}).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("SeamothModules");

		this.SetEquipment(EquipmentType.SeamothModule).WithQuickSlotType(QuickSlotType.Passive);

		if (GetBuilderIndex(TechType.VehicleHullModule3, out var group, out var category, out _))
			this.SetUnlock(TechType.BaseUpgradeConsole).WithPdaGroupCategoryAfter(group, category, TechType.VehicleHullModule3).WithAnalysisTech(null);

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
#endif