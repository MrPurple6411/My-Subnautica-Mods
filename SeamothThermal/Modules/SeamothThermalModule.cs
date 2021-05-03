namespace SeamothThermal.Modules
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using System.Collections.Generic;
    using UnityEngine;

    public class SeamothThermalModule: Equipable
    {
        public SeamothThermalModule() : base(
            "SeamothThermalModule",
            "Seamoth thermal reactor",
            "Recharges power cells in hot areas. Doesn't stack.")
        {
        }

        public override EquipmentType EquipmentType => EquipmentType.SeamothModule;

        public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;

        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;

        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;

        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;

        public override string[] StepsToFabricatorTab => new string[] { "SeamothModules" };

        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;

        public override GameObject GetGameObject()
        {
            // Get the ElectricalDefense module prefab and instantiate it
            string path = "WorldEntities/Tools/SeamothElectricalDefense";
            GameObject prefab = Resources.Load<GameObject>(path);
            GameObject obj = GameObject.Instantiate(prefab);

            // Get the TechTags and PrefabIdentifiers
            TechTag techTag = obj.GetComponent<TechTag>();
            PrefabIdentifier prefabIdentifier = obj.GetComponent<PrefabIdentifier>();

            // Change them so they fit to our requirements.
            techTag.type = TechType;
            prefabIdentifier.ClassId = ClassID;

            return obj;
        }
#if SN1
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Kyanite, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.WiringKit, 1)
                }
            };
        }
        protected override Atlas.Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.ExosuitThermalReactorModule);
        }
#elif BZ
        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Kyanite, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.WiringKit, 1)
                }
            };
        }
        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.ExosuitThermalReactorModule);
        }
#endif
    }
}
