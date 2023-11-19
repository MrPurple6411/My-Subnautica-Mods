namespace UnknownName.Patches;

using HarmonyLib;

[HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.CraftRecipe))]
public class TooltipFactory_Recipe
{
    [HarmonyPostfix]
    public static void Postfix(TechType techType, bool locked, ref TooltipData data)
    {
        if (locked && !CrafterLogic.IsCraftRecipeUnlocked(techType))
        {
            data.prefix.Clear();
            TooltipFactory.WriteTitle(data.prefix, Main.Config.UnKnownTitle);
            TooltipFactory.WriteDescription(data.prefix, Main.Config.UnKnownDescription);
        }
    }
}