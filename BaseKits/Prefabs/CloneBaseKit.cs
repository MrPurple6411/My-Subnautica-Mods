using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseKits.Prefabs
{
    internal class CloneBaseKit : Craftable
    {
        private static readonly List<TechType> UnlockRequired = new List<TechType>() { TechType.BaseRoom, TechType.BaseMapRoom, TechType.BaseMoonpool, TechType.BaseBioReactor, TechType.BaseNuclearReactor, TechType.BaseObservatory, TechType.BaseUpgradeConsole, TechType.BaseFiltrationMachine, TechType.BaseWaterPark };
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
            if (processedPrefab != null)
            {
                return GameObject.Instantiate(processedPrefab);
            }

            GameObject prefab = Utils.CreateGenericLoot(this.TechType);

            if (prefab != null)
            {
                processedPrefab = GameObject.Instantiate(prefab);
                prefab.SetActive(false);
            }

            return GameObject.Instantiate(processedPrefab);
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            if (processedPrefab != null)
            {
                gameObject.Set(GameObject.Instantiate(processedPrefab));
                yield break;
            }

            GameObject prefab = Utils.CreateGenericLoot(this.TechType);
            if (prefab != null)
            {
                processedPrefab = GameObject.Instantiate(prefab);
                prefab.SetActive(false);
            }

            gameObject.Set(GameObject.Instantiate(processedPrefab));
            yield break;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return CraftDataHandler.GetTechData(TypeToClone);
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return SpriteManager.Get(TypeToClone);
        }
    }
}
