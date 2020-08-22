using HarmonyLib;

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Analyze))]
    public class KnownTech_Analyze
    {
        [HarmonyPrefix]
        public static bool Prefix(TechType techType, bool verbose)
        {
            if (Main.config.Hardcore)
            {
                PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
                return !verbose || entryData == null || (entryData != null && PDAScanner.ContainsCompleteEntry(techType));
            }

            return true;
        }
    }

}