using System.Collections.Generic;
using HarmonyLib;
#if SUBNAUTICA
#elif BELOWZERO
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace UnKnownName
{
    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Initialize))]
    public class KnownTech_Initialize
    {
        [HarmonyPrefix]
        public static void Prefix(PDAData data)
        {
            List<TechType> types = data.defaultTech;
            List<TechType> removals = new List<TechType>();

            foreach (TechType techType in types)
            {
#if SUBNAUTICA
                if (techType == TechType.Titanium || CraftData.Get(techType, true) == null)
                {
#elif BELOWZERO
                if(techType == TechType.Titanium || TechData.GetIngredients(techType) == null)
                {
#endif
                    removals.Add(techType);
                }
            }

            foreach (TechType tech in removals)
            {
                data.defaultTech.Remove(tech);
            }
        }
    }

}