using System.Text;
using HarmonyLib;

namespace UnKnownName
{
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

}