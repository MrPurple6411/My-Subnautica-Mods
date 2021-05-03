namespace UnKnownName.Patches
{
    using System.Collections.Generic;
    using HarmonyLib;
    using QModManager.API;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Crafting;
#if SN1
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif


    [HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.IsCraftRecipeUnlocked))]
    public class CrafterLogic_IsCraftRecipeUnlocked
    {
        public static List<TechType> blackList = new List<TechType>() { TechType.Titanium, TechType.Copper };

        [HarmonyPostfix]
        public static void Postfix(TechType techType, ref bool __result)
        {
            if(Main.Config.Hardcore && GameModeUtils.RequiresBlueprints() && __result)
            {
                if(!QModServices.Main.ModPresent("UITweaks"))
                {
                    RecipeData data = CraftDataHandler.GetTechData(techType);
                    int ingredientCount = data?.ingredientCount ?? 0;
                    for(int i = 0; i < ingredientCount; i++)
                    {
                        Ingredient ingredient = data.Ingredients[i];
                        if(!blackList.Contains(techType) && !CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
                        {
                            __result = false;
                            return;
                        }
                    }
                }
                else
                {
#if SN1
                    if(CraftData.techData.TryGetValue(techType, out CraftData.TechData data))
                    {
                        int ingredientCount = data?.ingredientCount ?? 0;
                        for(int i = 0; i < ingredientCount; i++)
                        {
                            IIngredient ingredient = data.GetIngredient(i);
                            if(!blackList.Contains(techType) && !CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
                            {
                                __result = false;
                                return;
                            }
                        }
                    }
#elif BZ

#endif
                }
            }
        }
    }
}