using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using UnityEngine;
#if SUBNAUTICA
using Data = SMLHelper.V2.Crafting.TechData;
using Sprite = Atlas.Sprite;
#elif BELOWZERO
using Data = SMLHelper.V2.Crafting.RecipeData;
#endif

namespace CustomHullPlates.HullPlate
{
    public class BasicHullPlatePrefab : Buildable
    {
        private readonly Texture2D hullPlateIcon;
        private readonly Texture2D hullPlateTexture;

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
            _GameObject.name = this.ClassID;
            return _GameObject;
        }

        /// <summary>
        /// This provides the <see cref="Data"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected override Data GetBlueprintRecipe()
        {
            return new Data(new Ingredient(TechType.Titanium, 1), new Ingredient(TechType.Glass, 1));
        }

        /// <summary>
        /// This Provides the <see cref="Sprite"/> to assign to this GameObject.
        /// </summary>
        /// <returns></returns>
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(hullPlateIcon);
        }
    }
}
