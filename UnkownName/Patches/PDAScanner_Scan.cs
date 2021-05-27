namespace UnKnownName.Patches
{
    using HarmonyLib;
    using System;

    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Scan), new Type[] { })]
    public class PDAScanner_Scan
    {
        public static TechType techType;
        [HarmonyPrefix]
        public static void Prefix()
        {
            if(PDAScanner.scanTarget.techType != TechType.None)
            {
                techType = PDAScanner.scanTarget.techType;
            }
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            if(PDAScanner.ContainsCompleteEntry(techType) || KnownTech.Contains(techType))
            {
                if(techType == TechType.ScrapMetal && !KnownTech.Contains(TechType.Titanium))
                {
                    PDAScanner.AddByUnlockable(TechType.Titanium, 1);
                    KnownTech.Add(TechType.Titanium);
                }

                if(!KnownTech.Contains(techType))
                {
                    KnownTech.Add(techType);
                }

                var entryData = PDAScanner.GetEntryData(techType);

                if(entryData != null && entryData.locked)
                {
                    PDAScanner.Unlock(entryData, true, true);

                    if(!KnownTech.Contains(entryData.blueprint))
                    {
                        KnownTech.Add(entryData.blueprint);
                    }
                }
#if SN1
                var techType2 = CraftData.GetHarvestOutputData(techType);
#elif BZ
                var techType2 = TechData.GetHarvestOutput(techType);
#endif
                if(techType2 != TechType.None)
                {
                    if(!KnownTech.Contains(techType2))
                    {
                        KnownTech.Add(techType2);
                    }

                    var entryData2 = PDAScanner.GetEntryData(techType2);
                    if(entryData2 != null && entryData2.locked)
                    {
                        PDAScanner.Unlock(entryData, true, true);

                        if(!KnownTech.Contains(entryData2.blueprint))
                        {
                            KnownTech.Add(entryData2.blueprint);
                        }
                    }
                }
            }
        }
    }

}