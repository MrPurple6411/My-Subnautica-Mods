using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

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
                List<TechType> techTypes = new List<TechType> { TechType.Titanium, TechType.Copper, TechType.Quartz, TechType.Silver, TechType.Gold, TechType.Diamond, TechType.Lead, TechType.CreepvineSeedCluster, TechType.JellyPlant, TechType.JeweledDiskPiece, TechType.CreepvinePiece, TechType.AluminumOxide, TechType.Nickel, TechType.Kyanite, TechType.UraniniteCrystal, TechType.MercuryOre };
#if SUBNAUTICA
                ITechData data = CraftData.Get(techType, true);
                if (!techTypes.Contains(techType) && data != null)
                {
                    int ingredientCount = data.ingredientCount;
                    for (int i = 0; i < ingredientCount; i++)
                    {
                        IIngredient ingredient = data.GetIngredient(i);
                        if (!CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType) || (PDAScanner.GetEntryData(ingredient.techType)?.locked ?? false) || !KnownTech.Contains(ingredient.techType))
                        {
                            __result = false;
                            return;
                        }
                    }
                }
                if ((PDAScanner.GetEntryData(techType)?.locked ?? false) || !KnownTech.Contains(techType))
                {
                    __result = false;
                    return;
                }
#elif BELOWZERO
                if(!techTypes.Contains(techType))
                {
                    List<Ingredient> data = TechData.GetIngredients(techType)?.ToList() ?? new List<Ingredient>();
                    int ingredientCount = data.Count;
                    for(int i = 0; i < ingredientCount; i++)
                    {
                        Ingredient ingredient = data[i];
                        if(!CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
                        {
                            __result = false;
                            return;
                        }
                    }
                }
#endif
            }
        }
    }

}