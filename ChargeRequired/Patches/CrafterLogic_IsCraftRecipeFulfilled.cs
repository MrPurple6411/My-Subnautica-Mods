

namespace ChargeRequired.Patches;

using HarmonyLib;
using System.Linq;
#if BELOWZERO
using System.Collections.Generic;
#endif

[HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.IsCraftRecipeFulfilled))]
internal class CrafterLogic_IsCraftRecipeFulfilled
{
    [HarmonyPostfix]
    public static void CrafterLogic_IsCraftRecipeFulfilled_Postfix(TechType techType, ref bool __result)
    {
        if (!__result ||
#if SUBNAUTICA
			!GameModeUtils.RequiresIngredients()
#else
			!GameModeManager.GetOption<bool>(GameOption.CraftingRequiresResources)
#endif
			) return;
        var main = Inventory.main;
#if SUBNAUTICA
        var techData = CraftData.Get(techType, true);
        if(techData != null)
        {
            var i = 0;
            var ingredientCount = techData.ingredientCount;
            while(i < ingredientCount)
            {
                var ingredient = techData.GetIngredient(i);
#elif BELOWZERO
            IList<Ingredient> ingredients = TechData.GetIngredients(techType);
            if(ingredients != null)
            {
                var i = 0;
                var ingredientCount = ingredients.Count;
                while(i < ingredientCount)
                {
                    var ingredient = ingredients[i];
#endif
                var count = 0;
                var inventoryItems = main.container.GetItems(ingredient.techType);
                if(inventoryItems != null)
                {
                    count += inventoryItems.Count(inventoryItem => Main.BatteryCheck(inventoryItem.item));
                }
                if(count < ingredient.amount)
                {
                    __result = false;
                    return;
                }
                i++;
            }
            __result = true;
            return;
        }
        __result = false;
    
}
}
