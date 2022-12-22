namespace UnknownName.Patches
{
    using HarmonyLib;
    
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.CraftRecipe))]
    public class TooltipFactory_Recipe
    {
        [HarmonyPostfix]
        public static void Postfix(bool locked, ref TooltipData data)
        {
            if (locked && GameModeUtils.RequiresBlueprints())
            {
                data.prefix.Clear();
                TooltipFactory.WriteTitle(data.prefix, Main.SMLConfig.UnKnownTitle);
                TooltipFactory.WriteDescription(data.prefix, Main.SMLConfig.UnKnownDescription);
            }
        }
    }
}