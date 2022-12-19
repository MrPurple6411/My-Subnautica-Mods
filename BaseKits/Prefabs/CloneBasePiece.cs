namespace BaseKits.Prefabs
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
#if SN1
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#endif


    internal class CloneBasePiece : Buildable
    {
        private static readonly List<TechType> UnlockRequired = new()
        {
            TechType.BaseBulkhead, TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool,
            TechType.BaseBioReactor, TechType.BaseNuclearReactor, TechType.BaseObservatory, TechType.BaseUpgradeConsole,
            TechType.BaseFiltrationMachine, TechType.BaseWaterPark
        };

        private GameObject processedPrefab;
        private readonly TechType TypeToClone;
        private readonly TechType KitTechType;
        private readonly TechGroup group;
        private readonly TechCategory category;

        internal CloneBasePiece(TechType typeToClone, TechType kitTechType) : base($"CBP_{typeToClone}",
            $"{Language.main.Get(typeToClone)}", "Built from a Kit!")
        {
            TypeToClone = typeToClone;
            KitTechType = kitTechType;

            CraftData.GetBuilderIndex(typeToClone, out group, out category, out _);
            OnFinishedPatching += () =>
            {
                CraftDataHandler.SetBackgroundType(this.TechType, CraftData.BackgroundType.PlantAir);
            };
        }

        public override TechType RequiredForUnlock =>
            UnlockRequired.Contains(TypeToClone) ? TypeToClone : TechType.None;

        public override TechGroup GroupForPDA => group;

        public override TechCategory CategoryForPDA => category;


        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            GameObject go = null;
            if (processedPrefab != null)
            {
                go = Object.Instantiate(processedPrefab);
                go.SetActive(true);
                gameObject.Set(go);
                yield break;
            }

            var task = CraftData.GetPrefabForTechTypeAsync(TypeToClone);
            yield return task;

            processedPrefab = task.GetResult();
            go = Object.Instantiate(processedPrefab);
            go.SetActive(true);
            gameObject.Set(go);
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new() {craftAmount = 1, Ingredients = new List<Ingredient>() {new(KitTechType, 1)}};
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TypeToClone);
        }
    }
}