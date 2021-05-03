namespace BaseKits.Prefabs
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using System.Collections;
    using System.Collections.Generic;
#if SN1
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#else
    using SMLHelper.V2.Crafting;
#endif
    using UnityEngine;

    internal class CloneBaseKit: Craftable
    {
        private static readonly List<TechType> UnlockRequired = new List<TechType>() { TechType.BaseBulkhead, TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool, TechType.BaseBioReactor, TechType.BaseNuclearReactor, TechType.BaseObservatory, TechType.BaseUpgradeConsole, TechType.BaseFiltrationMachine, TechType.BaseWaterPark };
        private GameObject processedPrefab;
        private readonly TechType TypeToClone;

        internal CloneBaseKit(TechType typeToClone) : base($"Kit_{typeToClone}", $"{Language.main.Get(typeToClone)} Kit", "Super Compressed Base in a Kit")
        {
            TypeToClone = typeToClone;
        }

        public override TechType RequiredForUnlock => UnlockRequired.Contains(TypeToClone) ? TypeToClone : TechType.None;

        public override TechGroup GroupForPDA => TechGroup.Resources;

        public override TechCategory CategoryForPDA => TechCategory.AdvancedMaterials;

        public override float CraftingTime => 10f;

        public override GameObject GetGameObject()
        {
            GameObject go;
            if(processedPrefab != null)
            {
                go = GameObject.Instantiate(processedPrefab);
                go.SetActive(true);
                return go;
            }

            GameObject prefab = Utils.CreateGenericLoot(TechType);

            if(prefab != null)
            {
                processedPrefab = GameObject.Instantiate(prefab);
                processedPrefab.SetActive(false);
            }

            go = GameObject.Instantiate(processedPrefab);
            go.SetActive(true);
            return go;
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            GameObject go;
            if(processedPrefab != null)
            {
                go = GameObject.Instantiate(processedPrefab);
                go.SetActive(true);
                gameObject.Set(go);
                yield break;
            }

            GameObject prefab = Utils.CreateGenericLoot(TechType);
            if(prefab != null)
            {
                processedPrefab = GameObject.Instantiate(prefab);
                processedPrefab.SetActive(false);
            }

            go = GameObject.Instantiate(processedPrefab);
            go.SetActive(true);
            gameObject.Set(go);
            yield break;
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return CraftDataHandler.GetTechData(TypeToClone);
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TypeToClone);
        }
    }
}
