using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
    public class TooltipFactory_InventoryItem
    {
        [HarmonyPostfix]
        public static void Postfix(ref StringBuilder sb, TechType techType)
        {
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
            if (entryData == null || PDAScanner.ContainsCompleteEntry(techType) || CrafterLogic.IsCraftRecipeUnlocked(techType))
            {
                return;
            }
            
            sb.Clear();
            TooltipFactory.WriteTitle(sb, Main.config.UnKnownTitle);
            TooltipFactory.WriteDescription(sb, Main.config.UnKnownDescription);
        }
    }

}