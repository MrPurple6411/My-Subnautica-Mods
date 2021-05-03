namespace CustomHullPlates.HullPlate
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Utility;
    using System.Collections;
    using UnityEngine;
#if SN1
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#endif



    public class BasicHullPlatePrefab: Buildable
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

#if SUBNAUTICA_STABLE
        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.DioramaHullPlate);

            GameObject _GameObject = GameObject.Instantiate(prefab);

            MeshRenderer meshRenderer = _GameObject.FindChild("Icon").GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = hullPlateTexture;
            _GameObject.name = ClassID;

            return _GameObject;
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.DioramaHullPlate);
            yield return task;

            GameObject _GameObject = GameObject.Instantiate(task.GetResult());

            MeshRenderer meshRenderer = _GameObject.FindChild("Icon").GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = hullPlateTexture;
            _GameObject.name = ClassID;

            gameObject.Set(_GameObject);
            yield break;
        }

        /// <summary>
        /// This provides the <see cref="RecipeData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData(new Ingredient(TechType.Titanium, 1), new Ingredient(TechType.Glass, 1));
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
