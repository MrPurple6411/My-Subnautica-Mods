namespace UnKnownName.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;

    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Initialize))]
    public class KnownTech_Initialize
    {
        [HarmonyPrefix]
        public static void Prefix(PDAData data)
        {
            var types = new List<TechType>(data.defaultTech);
            foreach(var techType in types)
            {
                var entryData = PDAScanner.GetEntryData(techType);
                if(entryData != null && entryData.locked)
                {
                    data.defaultTech.Remove(techType);
                }
            }
        }
    }
}