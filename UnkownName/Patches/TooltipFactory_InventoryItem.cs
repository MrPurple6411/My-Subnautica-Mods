namespace UnKnownName.Patches
{
    using HarmonyLib;
    using System.Text;

    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
    public class TooltipFactory_InventoryItem
    {
        [HarmonyPostfix]
        public static void Postfix(ref StringBuilder sb, TechType techType)
        {
            var entryData = PDAScanner.GetEntryData(techType);
            if(entryData == null || PDAScanner.ContainsCompleteEntry(techType) || CrafterLogic.IsCraftRecipeUnlocked(techType))
            {
                return;
            }

            sb.Clear();
            TooltipFactory.WriteTitle(sb, Main.Config.UnKnownTitle);
            TooltipFactory.WriteDescription(sb, Main.Config.UnKnownDescription);
        }
    }

}