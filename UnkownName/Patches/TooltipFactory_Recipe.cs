using System.Text;
using HarmonyLib;

namespace UnKnownName.Patches
{
#if BELOWZERO_EXP
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.CraftRecipe))]
    public class TooltipFactory_Recipe
    {
        [HarmonyPostfix]
        public static void Postfix(bool locked, ref TooltipData data)
        {
            if (locked && GameModeUtils.RequiresBlueprints())
            {
                data.prefix.Clear();
                TooltipFactory.WriteTitle(data.prefix, Main.config.UnKnownTitle);
                TooltipFactory.WriteDescription(data.prefix, Main.config.UnKnownDescription);
            }
        }
    }
#else
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.Recipe))]
    public class TooltipFactory_Recipe
    {
        [HarmonyPostfix]
        public static void Postfix(bool locked, ref string tooltipText)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (locked && GameModeUtils.RequiresBlueprints())
            {
                TooltipFactory.WriteTitle(stringBuilder, Main.config.UnKnownTitle);
                TooltipFactory.WriteDescription(stringBuilder, Main.config.UnKnownDescription);
                tooltipText = stringBuilder.ToString();
            }
        }
    }
#endif
}