using FMOD;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace CustomHullPlates
{
    public class BasicPostersPrefab: Equipable
    {
        readonly Texture2D posterIcon;
        readonly Texture2D posterTexture;
        readonly string orientation;

        public BasicPostersPrefab(string classId, string friendlyName, string description, string orientation, Texture2D posterIcon, Texture2D posterTexture) : base(classId, friendlyName, description)
        {
            this.orientation = orientation;
            this.posterIcon = posterIcon;
            this.posterTexture = posterTexture;
        }

        public override TechGroup GroupForPDA => TechGroup.Resources;

        public override TechCategory CategoryForPDA => TechCategory.BasicMaterials;

        public override EquipmentType EquipmentType => EquipmentType.Hand;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;

        public override string[] StepsToFabricatorTab => orientation.ToLower() == "landscape" ? new string[] { "Posters", "Landscape" } : new string[] { "Posters", "Portrait" };

        public override GameObject GetGameObject()
        {
            GameObject prefab = this.orientation.ToLower() == "landscape"
                ? CraftData.GetPrefabForTechType(TechType.PosterAurora)
                : CraftData.GetPrefabForTechType(TechType.PosterKitty);

            GameObject _GameObject = UnityEngine.Object.Instantiate(prefab);
            _GameObject.name = ClassID;

            Material material = _GameObject.GetComponentInChildren<MeshRenderer>().materials[1];
            material.SetTexture("_MainTex", posterTexture);
            material.SetTexture("_SpecTex", posterTexture);

            return _GameObject;
        }

#if SUBNAUTICA
        /// <summary>
        /// This provides the <see cref="TechData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData() { 
                craftAmount = 1, 
                Ingredients = new List<Ingredient>(){
                    new Ingredient(TechType.Titanium, 1), 
                    new Ingredient(TechType.FiberMesh, 1) 
                } 
            };
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(posterIcon);
        }
#elif BELOWZERO
        /// <summary>
        /// This provides the <see cref="RecipeData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData(){ 
                craftAmount = 1, 
                Ingredients = new List<Ingredient>(){ 
                    new Ingredient(TechType.Titanium, 1), 
                    new Ingredient(TechType.FiberMesh, 1) 
                }
            };
        }
        
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(posterIcon);
        }
#endif

    }
}
