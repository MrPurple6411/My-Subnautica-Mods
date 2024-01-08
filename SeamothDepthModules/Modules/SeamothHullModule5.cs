#if SUBNAUTICA
namespace MoreSeamothDepth.Modules;

using System.Collections;
using Nautilus.Assets;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets.Gadgets;
using static CraftData;

public class SeamothHullModule5 : CustomPrefab
{
	[SetsRequiredMembers]
	public SeamothHullModule5() : base("SeamothHullModule5", "Seamoth depth module MK5", "Enhances diving depth to maximum. Does not stack.", SpriteManager.Get(TechType.VehicleHullModule3))
	{
		this.SetRecipe(new()
		{
			Ingredients = new List<Ingredient>()
				{
					new(Main.moduleMK4, 1),
					new(TechType.Titanium, 5),
					new(TechType.Lithium, 2),
					new(TechType.Kyanite, 4),
					new(TechType.Aerogel, 2)
				},
			craftAmount = 1
		}).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("SeamothDepthModules");

		this.SetEquipment(EquipmentType.SeamothModule).WithQuickSlotType(QuickSlotType.Passive);

		if (GetBuilderIndex(TechType.VehicleHullModule3, out var group, out var category, out _))
			this.SetUnlock(TechType.BaseUpgradeConsole).WithPdaGroupCategoryAfter(group, category, Main.moduleMK4).WithAnalysisTech(null, null, null);

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
		obj.SetActive(false);

		gameObject.Set(obj);
	}
}
#endif