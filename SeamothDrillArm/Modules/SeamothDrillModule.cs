using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;

namespace SeamothDrillArm.Modules
{
    public class SeamothDrillModule : SeamothModule
    {
        public SeamothDrillModule() : 
            base("SeamothDrillModule", 
                "Seamoth drill module", 
                "Enables the Seamoth to mine resources like the PRAWN Drill Arm.",
                CraftTree.Type.SeamothUpgrades, 
                new string[1] { "SeamothModules" }, 
                TechType.ExosuitDrillArmModule, 
                TechType.ExosuitDrillArmModule)
        {
            SeamothDrillModule = TechType;

            CraftDataHandler.SetQuickSlotType(SeamothDrillModule, QuickSlotType.Selectable);
        }

        public override TechData GetTechData()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.ExosuitDrillArmModule, 1),
                    new Ingredient(TechType.Diamond, 2),
                    new Ingredient(TechType.Titanium, 2)
                }
            };
        }
    }
}
