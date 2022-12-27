using System.Collections;

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
        }

        public override EquipmentType EquipmentType => EquipmentType.SeamothModule;

        public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;

        public override TechGroup GroupForPDA => TechGroup.Workbench;

        public override TechCategory CategoryForPDA => TechCategory.Workbench;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;

        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;


        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var taskResult = new TaskResult<GameObject>();
            yield return CraftData.InstantiateFromPrefabAsync(TechType.SeamothElectricalDefense, taskResult);
            var obj = taskResult.Get();
            
            // Get the TechTags and PrefabIdentifiers
            var techTag = obj.GetComponent<TechTag>();
            var prefabIdentifier = obj.GetComponent<PrefabIdentifier>();

            // Change them so they fit to our requirements.
            techTag.type = TechType;
            prefabIdentifier.ClassId = ClassID;
            gameObject.Set(obj);
        }


        protected override TechData GetBlueprintRecipe()
        {
            return new()
            {
                Ingredients = new List<Ingredient>()
                {
                    new(TechType.VehicleHullModule3, 1),
                    new(TechType.PlasteelIngot, 1),
                    new(TechType.Nickel, 2),
                    new(TechType.AluminumOxide, 3)
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