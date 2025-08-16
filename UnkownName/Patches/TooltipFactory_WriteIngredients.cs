using UnknownName;

namespace UnKnownName.Patches;

using HarmonyLib;
using System.Collections.Generic;
using static CraftData;

[HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.WriteIngredients))]
public class TooltipFactory_WriteIngredients
{

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
                KnownTech.Add(techType, true);
            }

            bool requiresUnlocking =
#if BELOWZERO
                GameModeManager.GetOption<bool>(GameOption.TechRequiresUnlocking);
#else
                GameModeUtils.RequiresBlueprints();
#endif
            if (KnownTech.Contains(techType) || !requiresUnlocking) continue;
            var icon = icons.Find((TooltipIcon) => TooltipIcon.sprite == SpriteManager.Get(techType) && TooltipIcon.text.Contains(Language.main.GetOrFallback(TooltipFactory.techTypeIngredientStrings.Get(techType), techType)));
            if (!icons.Contains(icon)) continue;
            icons.Remove(icon);
            var tooltipIcon = new TooltipIcon() { sprite = SpriteManager.Get(TechType.None), text = Main.Config.UnKnownTitle };
            icons.Add(tooltipIcon);
        }
    }
}