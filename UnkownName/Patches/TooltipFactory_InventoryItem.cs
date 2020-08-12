using System.Text;
using HarmonyLib;
using UnityEngine;

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
    public class TooltipFactory_InventoryItem
    {
        [HarmonyPostfix]
        public static void Postfix(ref StringBuilder sb, TechType techType, GameObject obj)
        {
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
            if (!PDAScanner.CanScan(obj) || PDAScanner.ContainsCompleteEntry(techType) || KnownTech.Contains(techType) || entryData == null)
            {
                return;
            }
            sb.Clear();
            TooltipFactory.WriteTitle(sb, Main.config.UnKnownTitle);
            TooltipFactory.WriteDescription(sb, Main.config.UnKnownDescription);
        }
    }

}