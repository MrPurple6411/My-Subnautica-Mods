namespace BuilderModule.Module
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Utility;
    using UnityEngine;

#if SN1
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#endif


    internal class BuilderModulePrefab: Equipable
    {
        private EquipmentType _equipmentType;
        private string[] _fabricatorPath;

        public BuilderModulePrefab(string classId,string friendlyName,string  description, string[] fabricatorPath, EquipmentType equipmentType) : base(classId, friendlyName, description)
        {
            _fabricatorPath = fabricatorPath;
            _equipmentType = equipmentType;
        }

        public static Sprite Sprite { get; } = ImageUtils.LoadSpriteFromFile($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Assets/BuilderModule.png");

        public override Vector2int SizeInInventory => new Vector2int(1, 1);

        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;

        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;

#if SN1
        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
#elif BZ
        public override CraftTree.Type FabricatorType => CraftTree.Type.SeaTruckFabricator;
#endif
        public override EquipmentType EquipmentType => _equipmentType;

        public override string[] StepsToFabricatorTab => _fabricatorPath;

        public override TechType RequiredForUnlock => TechType.Builder;

        public override QuickSlotType QuickSlotType => QuickSlotType.Toggleable;

#if SUBNAUTICA_STABLE
        public override GameObject GetGameObject()
        {
            GameObject gameobject = CraftData.GetPrefabForTechType(TechType.SeamothSonarModule, false);
            return GameObject.Instantiate(gameobject);
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.VehicleStorageModule, false);

            yield return task;
            GameObject prefab = GameObject.Instantiate(task.GetResult(), default, default, false);
            prefab.GetComponentsInChildren<UniqueIdentifier>().ForEach((x)=> { if(x is PrefabIdentifier) x.classId = ClassID; else GameObject.DestroyImmediate(x.gameObject); });
            if(prefab.TryGetComponent(out TechTag tag)) tag.type = TechType;
            GameObject.DestroyImmediate(prefab.GetComponent<SeamothStorageContainer>());

            gameObject.Set(prefab);

            yield break;
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[2]
                {
                    new Ingredient(TechType.Builder, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1)
                })
            };
        }

        protected override Sprite GetItemSprite()
        {
            return Sprite;
        }
    }
}