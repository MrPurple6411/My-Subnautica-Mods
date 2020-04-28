using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace CustomHullPlates
{
    public class BasicHullPlatePrefab: Buildable
    {
        readonly Texture2D hullPlateIcon;
        readonly Texture2D hullPlateTexture;

        public BasicHullPlatePrefab(string classId, string friendlyName, string description, Texture2D hullPlateIcon, Texture2D hullPlateTexture) : base(classId, friendlyName, description)
        {
            this.hullPlateIcon = hullPlateIcon;
            this.hullPlateTexture = hullPlateTexture;
        }

        public override TechGroup GroupForPDA => TechGroup.Miscellaneous;

        public override TechCategory CategoryForPDA => TechCategory.MiscHullplates;

        public override GameObject GetGameObject()
        {
            GameObject _GameObject = UnityEngine.Object.Instantiate(CraftData.GetPrefabForTechType(TechType.DioramaHullPlate));

            MeshRenderer meshRenderer = _GameObject.FindChild("Icon").GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = hullPlateTexture;
            _GameObject.name = ClassID;
            return _GameObject;
        }

#if SUBNAUTICA
        /// <summary>
        /// This provides the <see cref="TechData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData(new Ingredient(TechType.Titanium, 1), new Ingredient(TechType.Glass, 1));
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(hullPlateIcon);
        }
#elif BELOWZERO
        /// <summary>
        /// This provides the <see cref="RecipeData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData(new Ingredient(TechType.Titanium, 1), new Ingredient(TechType.Glass, 1));
        }
        
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(hullPlateIcon);
        }
#endif

    }
}
