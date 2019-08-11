using System.Collections.Generic;
using SMLHelper.V2.Crafting;

namespace MoreSeamothDepth.Modules
{
    public class SeamothHullModule4 : SeamothModule
    {
        public SeamothHullModule4() : 
            base("SeamothHullModule4", 
                "Seamoth depth module MK4", 
                "Enhances diving depth. Does not stack.", 
                CraftTree.Type.Workbench, 
                new string[1] { "SeamothMenu" }, 
                TechType.VehicleHullModule3, 
                TechType.VehicleHullModule3,
                SpriteManager.Get(TechType.VehicleHullModule3))
        {
            SeamothHullModule4 = TechType;
        }

        public override TechData GetTechData()
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
    }
}
