namespace CustomHullPlates.HullPlate;

using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static CraftData;

public class BasicHullPlatePrefab: CustomPrefab
{
    private readonly Texture2D _hullPlateTexture;

	[SetsRequiredMembers]
    public BasicHullPlatePrefab(string classId, string friendlyName, string description, Texture2D hullPlateIcon, Texture2D hullPlateTexture) : 
		base(classId, friendlyName, description, ImageUtils.LoadSpriteFromTexture(hullPlateIcon))
    {
		
		KnownTechHandler.UnlockOnStart(Info.TechType);
		_hullPlateTexture = hullPlateTexture;
		this.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.MiscHullplates).SetBuildable();
		this.SetRecipe(new(new Ingredient(TechType.Titanium, 1), new Ingredient(TechType.Glass, 1)));
		SetGameObject(GetGameObjectAsync);

		Register();
    }

    public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {
        var task = GetPrefabForTechTypeAsync(TechType.DioramaHullPlate);
        yield return task;

        var _GameObject = Object.Instantiate(task.GetResult());

        var meshRenderer = _GameObject.FindChild("Icon").GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = _hullPlateTexture;
        _GameObject.name = Info.ClassID;

        gameObject.Set(_GameObject);
    }
}
