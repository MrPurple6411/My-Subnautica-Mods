namespace BuilderModule.Module;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using UnityEngine;
using static CraftData;

internal class BuilderModulePrefab : CustomPrefab
{
	private static Texture2D SpriteTexture { get; } = ImageUtils.LoadTextureFromFile($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Assets/BuilderModule.png");

	[SetsRequiredMembers]
	public BuilderModulePrefab() : base("BuilderModule", "Builder Module", "Allows you to build bases while in your vehicle.")
	{
		if (SpriteTexture != null)
			Info.WithIcon(ImageUtils.LoadSpriteFromTexture(SpriteTexture));

		this.SetUnlock(TechType.BaseUpgradeConsole).WithAnalysisTech(null, null, null)
			.WithPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);

		this.SetRecipe(new()
		{
			craftAmount = 1,
			Ingredients = new List<Ingredient>(new Ingredient[]
			{
				new(TechType.Builder, 1),
				new(TechType.AdvancedWiringKit, 1)
			})
		})
#if SUBNAUTICA
			.WithFabricatorType(CraftTree.Type.SeamothUpgrades)
		.WithStepsToFabricatorTab(new[] { "CommonModules" })
#elif BELOWZERO
			.WithFabricatorType(CraftTree.Type.SeaTruckFabricator)
		.WithStepsToFabricatorTab(new[] { "ExosuitModules" })
#endif
		.WithCraftingTime(2f);

		this.SetEquipment(EquipmentType.VehicleModule).WithQuickSlotType(QuickSlotType.Toggleable);

		SetGameObject(GetGameObjectAsync);

		Register();
	}

	public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
	{
		var task = CraftData.GetPrefabForTechTypeAsync(TechType.VehicleStorageModule, false);

		yield return task;
		var prefab = EditorModifications.Instantiate(task.GetResult(), default, default, false);
		prefab.GetComponentsInChildren<UniqueIdentifier>().ForEach((x) => { if (x is PrefabIdentifier) x.classId = Info.ClassID; else Object.DestroyImmediate(x.gameObject); });
		if (prefab.TryGetComponent(out TechTag tag)) tag.type = Info.TechType;
		Object.DestroyImmediate(prefab.GetComponent<SeamothStorageContainer>());

		gameObject.Set(prefab);
	}
}