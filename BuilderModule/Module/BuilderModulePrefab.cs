using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using UnityEngine;

#if SUBNAUTICA
using RecipeData = SMLHelper.V2.Crafting.TechData;
using Sprite = Atlas.Sprite;
#endif


namespace BuilderModule.Module
{
    internal class BuilderModulePrefab : Equipable
    {
        public BuilderModulePrefab() : base("BuilderModule", "Builder Module", "Allows you to build bases while in your vehicle.")
        {
        }

        public override EquipmentType EquipmentType => EquipmentType.VehicleModule;

        public override Vector2int SizeInInventory => new Vector2int(1,1);

        public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;

        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;

        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;

        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;

        public override string[] StepsToFabricatorTab => new string[] { "CommonModules" };

        public override QuickSlotType QuickSlotType => QuickSlotType.Toggleable;

        public override GameObject GetGameObject()
        {
            return Object.Instantiate(CraftData.GetPrefabForTechType(TechType.SeamothSonarModule));
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
            return ImageUtils.LoadSpriteFromFile($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Assets/{ClassID}.png");
        }
    }
}