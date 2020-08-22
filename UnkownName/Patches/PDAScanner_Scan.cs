using System;
using HarmonyLib;

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Scan), new Type[] { })]
    public class PDAScanner_Scan
    {
        public static TechType techType;
        [HarmonyPrefix]
        public static void Prefix()
        {
            if (PDAScanner.scanTarget.techType != TechType.None)
            {
                techType = PDAScanner.scanTarget.techType;
            }
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (PDAScanner.ContainsCompleteEntry(techType) || KnownTech.Contains(techType))
            {
                if(!KnownTech.Contains(techType))
                    KnownTech.Add(techType);
                PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
                
                if (entryData != null && entryData.locked)
                {
                    PDAScanner.Unlock(entryData, true, true, true);

                    if (!KnownTech.Contains(entryData.blueprint))
                        KnownTech.Add(entryData.blueprint);
                }
#if SUBNAUTICA
                TechType techType2 = CraftData.GetHarvestOutputData(techType);
#elif BELOWZERO
                TechType techType2 = TechData.GetHarvestOutput(techType);
#endif
                if (techType2 != TechType.None)
                {
                    if (!KnownTech.Contains(techType2))
                        KnownTech.Add(techType2);
                    PDAScanner.EntryData entryData2 = PDAScanner.GetEntryData(techType2);
                    if (entryData2 != null && entryData2.locked)
                    {
                        PDAScanner.Unlock(entryData, true, true, true);

                        if (!KnownTech.Contains(entryData2.blueprint))
                            KnownTech.Add(entryData2.blueprint);
                    }
                }
            }
        }
    }

}