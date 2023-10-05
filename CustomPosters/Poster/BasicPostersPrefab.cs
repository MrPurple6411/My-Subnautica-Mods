
namespace CustomPosters.Poster;

using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static CraftData;

public class BasicPostersPrefab : CustomPrefab
{
    private readonly Texture2D _posterTexture;
    private readonly string _orientation;
    private static readonly int _mainTex = Shader.PropertyToID("_MainTex");
    private static readonly int _specTex = Shader.PropertyToID("_SpecTex");

	[SetsRequiredMembers]
    public BasicPostersPrefab(string classId, string friendlyName, string description, string orientation,
        Texture2D posterIcon, Texture2D posterTexture) : base(classId, friendlyName, description, ImageUtils.LoadSpriteFromTexture(posterIcon))
    {
		_orientation = orientation;
		_posterTexture = posterTexture;

		KnownTechHandler.UnlockOnStart(Info.TechType);
		var groupName = "Custom_Posters";
		if (!EnumHandler.TryGetValue(groupName, out TechGroup group))
		{
			group = EnumHandler.AddEntry<TechGroup>(groupName).WithPdaInfo($"Custom Posters");
		}

		var catagoryName = $"{orientation}_Posters";
		if (!EnumHandler.TryGetValue<TechCategory>(catagoryName, out var category))
		{
			category = EnumHandler.AddEntry<TechCategory>(catagoryName).WithPdaInfo($"{orientation} Posters").RegisterToTechGroup(group);
		}

		this.SetRecipe(new()
		{
			craftAmount = 1,
			Ingredients = new List<Ingredient>()
			{
				new(TechType.Titanium, 1),
				new(TechType.FiberMesh, 1)
			}
		}).WithFabricatorType(CraftTree.Type.Fabricator)
		.WithStepsToFabricatorTab(orientation.ToLower() == "landscape" ? new[] { "Posters", "Landscape" } : new[] { "Posters", "Portrait" });

		this.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

		SetGameObject(GetGameObjectAsync);

		Register();
    }

    public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {
        var task = _orientation.ToLower() == "landscape"
            ? CraftData.GetPrefabForTechTypeAsync(TechType.PosterAurora)
            : CraftData.GetPrefabForTechTypeAsync(TechType.PosterKitty);

        yield return task;

        var _GameObject = Object.Instantiate(task.GetResult());
        _GameObject.name = Info.ClassID;

        var material = _GameObject.GetComponentInChildren<MeshRenderer>().materials[1];
        material.SetTexture(_mainTex, _posterTexture);
        material.SetTexture(_specTex, _posterTexture);

        gameObject.Set(_GameObject);
    }
}