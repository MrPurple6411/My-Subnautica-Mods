using System.Collections.Generic;
using HarmonyLib;

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Initialize))]
    public class KnownTech_Initialize
    {
        [HarmonyPrefix]
        public static void Prefix(PDAData data)
        {
            List<TechType> types = new List<TechType>(data.defaultTech);
            foreach(TechType techType in types)
            {
                PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
                if (entryData != null && entryData.locked)
                {
                    data.defaultTech.Remove(techType);
                }
            }
        }
    }
}