using System;
using HarmonyLib;
#if SUBNAUTICA
#elif BELOWZERO
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace UnKnownName
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
            if (PDAScanner.ContainsCompleteEntry(techType) && !KnownTech.Contains(techType))
            {
                KnownTech.Add(techType, true);
#if SUBNAUTICA
                TechType techType2 = CraftData.GetHarvestOutputData(techType);
#elif BELOWZERO
                TechType techType2 = TechData.GetHarvestOutput(techType);
#endif
                if (techType2 != TechType.None)
                {
                    KnownTech.Add(techType2, true);
                }
            }
        }
    }

}