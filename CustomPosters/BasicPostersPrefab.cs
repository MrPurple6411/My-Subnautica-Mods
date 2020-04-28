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
    public class BasicPostersPrefab: Buildable
    {
        readonly Texture2D posterIcon;
        readonly Texture2D posterTexture;
        readonly string orientation;

        public BasicPostersPrefab(string classId, string friendlyName, string description, string orientation, Texture2D posterIcon, Texture2D posterTexture) : base(classId, friendlyName, description)
        {
            this.orientation = orientation;
            this.posterIcon = posterIcon;
            this.posterTexture = posterTexture;
            this.TechType = TechTypeHandler.AddTechType(classId, friendlyName, description);
        }

        public override TechGroup GroupForPDA => TechGroup.Miscellaneous;

        public override TechCategory CategoryForPDA => TechCategory.MiscHullplates;

        public override GameObject GetGameObject()
        {
            GameObject _GameObject;
            if(this.orientation.ToLower() == "landscape")
            {

                _GameObject = CraftData.InstantiateFromPrefab(TechType.PosterAurora);
            }
            else
            {
                _GameObject = CraftData.InstantiateFromPrefab(TechType.PosterKitty);
            }

            _GameObject.name = ClassID;

            Constructable constructable = _GameObject.EnsureComponent<Constructable>();
            constructable.allowedInBase = true;
            constructable.allowedInSub = true;
            constructable.allowedOnConstructables = true;
            constructable.allowedOnWall = true;
            constructable.allowedOutside = true;
            constructable.constructedAmount = 1;

            MeshRenderer meshRenderer = _GameObject.GetComponentInChildren<MeshRenderer>();
            foreach (Material material in meshRenderer.materials.Where((m)=> !m.name.Contains("magnet")) ?? new List<Material>())
            {
                Texture2D blankTexture = new Texture2D(posterTexture.width, posterTexture.height, TextureFormat.ARGB32, false);

                material.SetTexture("_MainTex", posterTexture);
                material.SetTexture("_SpecTex", blankTexture);
            }

            constructable.model = _GameObject;
            Pickupable pickupable = _GameObject.GetComponentInChildren<Pickupable>();
            pickupable.isPickupable = false;
            pickupable.overrideTechType = this.TechType;

            Main.customPosters.Add(ClassID);
            return _GameObject;
        }

#if SUBNAUTICA
        /// <summary>
        /// This provides the <see cref="TechData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData(new Ingredient(TechType.Titanium, 1));
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
            return new RecipeData(new Ingredient(TechType.Titanium, 1), new Ingredient(TechType.Glass, 1));
        }
        
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(posterIcon);
        }
#endif

    }
}
