#if SN1
namespace MoreSeamothDepth.Modules
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using System.Collections.Generic;
    using UnityEngine;

    public class SeamothHullModule4: Equipable
    {
        public SeamothHullModule4() : base(
            classId: "SeamothHullModule4",
            friendlyName: "Seamoth depth module MK4",
            description: "Enhances diving depth. Does not stack.")
        {
            OnStartedPatching += () => CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "EDM", "Enhanced Depth Modules", SpriteManager.Get(TechType.VehicleHullModule1));
        }

        public override EquipmentType EquipmentType => EquipmentType.SeamothModule;

        public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;

        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;

        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;

        public override string[] StepsToFabricatorTab => new string[] { "EDM" };

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
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.VehicleHullModule3, 1),
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.Nickel, 2),
                    new Ingredient(TechType.AluminumOxide, 3)
                },
                craftAmount = 1
            };
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.VehicleHullModule3);
        }
    }
}
#endif