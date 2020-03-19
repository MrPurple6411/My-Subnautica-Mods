using SMLHelper.V2.Crafting;
using System.Collections.Generic;

namespace MoreSeamothDepth.Modules
{
    public class SeamothHullModule5 : SeamothModule
    {
        public SeamothHullModule5() :
            base("SeamothHullModule5",
                "Seamoth depth module MK5",
                "Enhances diving depth to maximum. Does not stack.",
                CraftTree.Type.Workbench,
                new string[1] { "SeamothMenu" },
                SeamothHullModule4,
                SeamothHullModule4,
                SpriteManager.Get(TechType.VehicleHullModule3))
        {
            SeamothHullModule5 = TechType;
        }

        public override TechData GetTechData()
        {
            return new TechData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(SeamothHullModule4, 1),
                    new Ingredient(TechType.Titanium, 5),
                    new Ingredient(TechType.Lithium, 2),
                    new Ingredient(TechType.Kyanite, 4),
                    new Ingredient(TechType.Aerogel, 2)
                },
                craftAmount = 1
            };
        }
    }
}
