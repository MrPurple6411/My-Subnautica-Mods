namespace UnKnownName.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;

    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.WriteIngredients))]
    public class TooltipFactory_WriteIngredients
    {
#if SN1

        [HarmonyPostfix]
        public static void Postfix(ITechData data, ref List<TooltipIcon> icons)
        {
            if(data == null)
            {
                return;
            }
            var ingredientCount = data.ingredientCount;
            for(var i = 0; i < ingredientCount; i++)
            {
                var ingredient = data.GetIngredient(i);
                var techType = ingredient.techType;
                if(!KnownTech.Contains(techType) && PDAScanner.ContainsCompleteEntry(techType))
                {
                    KnownTech.Add(techType);
                    continue;
                }
                if(!CrafterLogic.IsCraftRecipeUnlocked(techType))
                {
                    var icon = icons.Find((TooltipIcon) => TooltipIcon.sprite == SpriteManager.Get(techType) && TooltipIcon.text.Contains(Language.main.GetOrFallback(TooltipFactory.techTypeIngredientStrings.Get(techType), techType)));
                    if(icons.Contains(icon))
                    {
                        icons.Remove(icon);
                        var tooltipIcon = new TooltipIcon() { sprite = SpriteManager.Get(TechType.None), text = Main.Config.UnKnownTitle };
                        icons.Add(tooltipIcon);
                    }
                }
            }
        }

#elif BZ

        [HarmonyPostfix]
        public static void Postfix(IList<Ingredient> ingredients, ref List<TooltipIcon> icons)
        {
            if (ingredients == null)
            {
                return;
            }

            var ingredientCount = ingredients.Count;
            for(var i = 0; i < ingredientCount; i++)
            {
                var techType = ingredients[i].techType;
                if(!KnownTech.Contains(techType) && PDAScanner.ContainsCompleteEntry(techType))
                {
                    KnownTech.Add(techType);
                }

                if (KnownTech.Contains(techType) || !GameModeUtils.RequiresBlueprints()) continue;
                var icon = icons.Find((TooltipIcon) => TooltipIcon.sprite == SpriteManager.Get(techType) && TooltipIcon.text.Contains(Language.main.GetOrFallback(TooltipFactory.techTypeIngredientStrings.Get(techType), techType)));
                if (!icons.Contains(icon)) continue;
                icons.Remove(icon);
                var tooltipIcon = new TooltipIcon() { sprite = SpriteManager.Get(TechType.None), text = Main.Config.UnKnownTitle };
                icons.Add(tooltipIcon);
            }
        }

#endif
    }

}