using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.IsCraftRecipeUnlocked))]
    public class CrafterLogic_IsCraftRecipeUnlocked
    {
        [HarmonyPostfix]
        public static void Postfix(TechType techType, ref bool __result)
        {
            if (Main.config.Hardcore && GameModeUtils.RequiresBlueprints() && __result)
            {
                RecipeData data = Main.GetData(techType);
                int ingredientCount = data?.ingredientCount ?? 0;
                for (int i = 0; i < ingredientCount; i++)
                {
                    Ingredient ingredient = data.Ingredients[i];
                    if (ingredient.techType != TechType.ScrapMetal && !CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
                    {
                        __result = false;
                        return;
                    }
                }
            }
        }
    }
}