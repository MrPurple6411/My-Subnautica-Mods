using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseKits.Prefabs
{
    internal class CloneBasePiece : Buildable
    {
        private static readonly List<TechType> UnlockRequired = new List<TechType>() { TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool, TechType.BaseBioReactor, TechType.BaseNuclearReactor, TechType.BaseObservatory, TechType.BaseUpgradeConsole, TechType.BaseFiltrationMachine, TechType.BaseWaterPark };
        private GameObject processedPrefab;
        private readonly TechType TypeToClone;
        private readonly TechType KitTechType;
        private readonly TechGroup group;
        private readonly TechCategory category;

        internal CloneBasePiece(TechType typeToClone, TechType kitTechType) : base($"CBP_{typeToClone}", $"{Language.main.Get(typeToClone)}", "Built from a Kit!")
        {
            TypeToClone = typeToClone;
            KitTechType = kitTechType;

            CraftData.GetBuilderIndex(typeToClone, out group, out category, out _);
        }

        public override TechType RequiredForUnlock => UnlockRequired.Contains(TypeToClone) ? TypeToClone : TechType.None;

        public override TechGroup GroupForPDA => group;

        public override TechCategory CategoryForPDA => category;

        public override GameObject GetGameObject()
        {
            if(processedPrefab != null)
                return GameObject.Instantiate(processedPrefab);

            GameObject prefab = CraftData.GetPrefabForTechType(TypeToClone);
            
            if (prefab != null)
            {
                processedPrefab = GameObject.Instantiate(prefab);
                prefab.SetActive(false);
            }

            return GameObject.Instantiate(processedPrefab);
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            if(processedPrefab != null)
            {
                gameObject.Set(processedPrefab);
                yield break;
            }

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TypeToClone);
            yield return task;

            GameObject prefab = task.GetResult();
            if (prefab != null)
            {
                processedPrefab = GameObject.Instantiate(prefab);
                prefab.SetActive(false);
            }

            gameObject.Set(processedPrefab);
            yield break;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData() { craftAmount = 1, Ingredients = new List<Ingredient>() { new Ingredient(KitTechType, 1) } };
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return SpriteManager.Get(TypeToClone);
        }
    }
}
