namespace UnKnownName.Patches
{
    using HarmonyLib;
    
#if BZ
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.CraftRecipe))]
    public class TooltipFactory_Recipe
    {
        [HarmonyPostfix]
        public static void Postfix(bool locked, ref TooltipData data)
        {
            if (locked && GameModeUtils.RequiresBlueprints())
            {
                data.prefix.Clear();
                TooltipFactory.WriteTitle(data.prefix, Main.SmcConfig.UnKnownTitle);
                TooltipFactory.WriteDescription(data.prefix, Main.SmcConfig.UnKnownDescription);
            }
        }
    }
#else
    using System.Text;    
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.Recipe))]
    public class TooltipFactory_Recipe
    {
        [HarmonyPostfix]
        public static void Postfix(bool locked, ref string tooltipText)
        {
            var stringBuilder = new StringBuilder();
            if (!locked || !GameModeUtils.RequiresBlueprints()) return;
            TooltipFactory.WriteTitle(stringBuilder, Main.SmcConfig.UnKnownTitle);
            TooltipFactory.WriteDescription(stringBuilder, Main.SmcConfig.UnKnownDescription);
            tooltipText = stringBuilder.ToString();
        }
    }
#endif
}