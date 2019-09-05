using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;

namespace SeamothClawArm.Modules
{
    public class SeamothClawModule : SeamothModule
    {
        public SeamothClawModule() :
            base("SeamothClawModule",
                "Seamoth claw module",
                "Enables the Seamoth to pick up resources like the PRAWN Claw Arm.",
                CraftTree.Type.SeamothUpgrades,
                new string[1] { "SeamothModules" },
                TechType.ExosuitDrillArmModule,
                TechType.ExosuitDrillArmModule)
        {
            SeamothClawModule = TechType;

            CraftDataHandler.SetQuickSlotType(SeamothClawModule, QuickSlotType.Selectable);
        }

        public override TechData GetTechData()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Titanium, 5),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Silicone, 1),
                    new Ingredient(TechType.Aerogel, 1)
                }
            };
        }
    }
}