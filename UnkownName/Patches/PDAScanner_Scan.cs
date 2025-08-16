namespace UnKnownName.Patches;

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
#if SUBNAUTICA
                KnownTech.Add(TechType.Titanium);
#elif BELOWZERO
                KnownTech.Add(TechType.Titanium, true);
#endif
            }

            if(!KnownTech.Contains(techType))
            {
#if SUBNAUTICA
                KnownTech.Add(techType);
#elif BELOWZERO
                KnownTech.Add(techType, true);
#endif
            }

            var entryData = PDAScanner.GetEntryData(techType);

            if(entryData != null && entryData.locked)
            {
                PDAScanner.Unlock(entryData, true, true);

                if(!KnownTech.Contains(entryData.blueprint))
                {
#if SUBNAUTICA
                    KnownTech.Add(entryData.blueprint);
#elif BELOWZERO
                    KnownTech.Add(entryData.blueprint,true);
#endif
                }
            }
            var techType2 = TechData.GetHarvestOutput(techType);
            if(techType2 != TechType.None)
            {
                if(!KnownTech.Contains(techType2))
                {
#if SUBNAUTICA
                    KnownTech.Add(techType2);
#elif BELOWZERO
                    KnownTech.Add(techType2, true);
#endif
                }

                var entryData2 = PDAScanner.GetEntryData(techType2);
                if(entryData2 != null && entryData2.locked)
                {
                    PDAScanner.Unlock(entryData, true, true);

                    if(!KnownTech.Contains(entryData2.blueprint))
                    {
#if SUBNAUTICA
                        KnownTech.Add(entryData2.blueprint);
#elif BELOWZERO
                        KnownTech.Add(entryData2.blueprint, true);
#endif
                    }
                }
            }
        }
    }
}