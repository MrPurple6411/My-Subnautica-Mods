namespace ChargeRequired.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;

    [HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.IsCraftRecipeFulfilled))]
    internal class CrafterLogic_IsCraftRecipeFulfilled
    {
        [HarmonyPostfix]
        public static void CrafterLogic_IsCraftRecipeFulfilled_Postfix(TechType techType, ref bool __result)
        {
            if(__result && GameModeUtils.RequiresIngredients())
            {
                Inventory main = Inventory.main;
#if SN1
                ITechData techData = CraftData.Get(techType, true);
                if(techData != null)
                {
                    int i = 0;
                    int ingredientCount = techData.ingredientCount;
                    while(i < ingredientCount)
                    {
                        IIngredient ingredient = techData.GetIngredient(i);
#elif BZ
                IList<Ingredient> ingredients = TechData.GetIngredients(techType);
                if(ingredients != null)
                {
                    int i = 0;
                    int ingredientCount = ingredients.Count;
                    while(i < ingredientCount)
                    {
                        Ingredient ingredient = ingredients[i];
#endif
                        int count = 0;
                        IList<InventoryItem> inventoryItems = main.container.GetItems(ingredient.techType);
                        if(inventoryItems != null)
                        {
                            foreach(InventoryItem inventoryItem in inventoryItems)
                            {
                                if(Main.BatteryCheck(inventoryItem.item))
                                {
                                    count++;
                                }
                            }
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
                return;
            }
        }
    }
}
