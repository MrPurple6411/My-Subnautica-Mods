using System.Collections.Generic;
using SMLHelper.V2.Crafting;

namespace BuilderModule
{
    internal class BuilderModulePrefab : Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal BuilderModulePrefab()
            : base(nameID: "BuilderModule",
                  nameUsingForFiles: "BuilderModule",
                  friendlyName: "Builder Module",
                  description: "Allows you to build bases while in a Seamoth.",
                  template: TechType.SeamothSonarModule,
                  fabricatorType: CraftTree.Type.SeamothUpgrades,
                  fabricatorTab: "CommonModules",
                  requiredAnalysis: TechType.BaseUpgradeConsole,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: EquipmentType.VehicleModule,
                  quickSlotType: QuickSlotType.Toggleable,
                  itemSize: new Vector2int(1, 1),
                  gamerResourceFileName: null
                  )
        {
        }

        public override void Patch()
        {
            base.Patch();
            TechTypeID = this.TechType;
        }

#if SUBNAUTICA

        protected override TechData GetRecipe()
        {
            return new TechData()
#elif BELOWZERO

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
#endif
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[2]
                {
                    new Ingredient(TechType.Builder, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1)
                })
            };
        }
    }
}