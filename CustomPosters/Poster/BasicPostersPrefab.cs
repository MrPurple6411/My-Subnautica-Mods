using SMLHelper.Handlers;

namespace CustomPosters.Poster
{
    using SMLHelper.Assets;
    using SMLHelper.Crafting;
    using SMLHelper.Utility;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BasicPostersPrefab : Equipable
    {
        private readonly Texture2D posterIcon;
        private readonly Texture2D posterTexture;
        private readonly string orientation;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int SpecTex = Shader.PropertyToID("_SpecTex");
        private readonly TechGroup group;
        private readonly TechCategory category;

        public BasicPostersPrefab(string classId, string friendlyName, string description, string orientation,
            Texture2D posterIcon, Texture2D posterTexture) : base(classId, friendlyName, description)
        {
            this.orientation = orientation;
            this.posterIcon = posterIcon;
            this.posterTexture = posterTexture;

            group = TechGroupHandler.AddTechGroup($"Custom_Posters", $"Custom Posters");
            category = TechCategoryHandler.AddTechCategory($"{orientation}_Posters", $"{orientation} Posters");
            TechCategoryHandler.TryRegisterTechCategoryToTechGroup(group, category);
        }

        public override TechGroup GroupForPDA => group;

        public override TechCategory CategoryForPDA => category;

        public override EquipmentType EquipmentType => EquipmentType.Hand;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;

        public override QuickSlotType QuickSlotType => QuickSlotType.Selectable;

        public override string[] StepsToFabricatorTab => orientation.ToLower() == "landscape"
            ? new[] {"Posters", "Landscape"}
            : new[] {"Posters", "Portrait"};

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = orientation.ToLower() == "landscape"
                ? CraftData.GetPrefabForTechTypeAsync(TechType.PosterAurora)
                : CraftData.GetPrefabForTechTypeAsync(TechType.PosterKitty);

            yield return task;

            var _GameObject = Object.Instantiate(task.GetResult());
            _GameObject.name = ClassID;

            var material = _GameObject.GetComponentInChildren<MeshRenderer>().materials[1];
            material.SetTexture(MainTex, posterTexture);
            material.SetTexture(SpecTex, posterTexture);

            gameObject.Set(_GameObject);
        }

#if SN1
        /// <summary>
        /// This provides the <see cref="TechData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected override TechData GetBlueprintRecipe()
        {
            return new()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new(TechType.Titanium, 1),
                    new(TechType.FiberMesh, 1)
                }
            };
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(posterIcon);
        }
#elif BZ
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