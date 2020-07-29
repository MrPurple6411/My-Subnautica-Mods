using System;
using System.Collections.Generic;
using HarmonyLib;
#if SUBNAUTICA
#elif BELOWZERO
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace UnKnownName
{
    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Initialize))]
    public class PDAScanner_Iniialize
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            List<TechType> techTypes = new List<TechType> { TechType.Titanium, TechType.Copper, TechType.Quartz, TechType.Silver, TechType.Gold, TechType.Diamond, TechType.Lead, TechType.CreepvineSeedCluster, TechType.JellyPlant, TechType.JeweledDiskPiece, TechType.CreepvinePiece, TechType.AluminumOxide, TechType.Nickel, TechType.Kyanite, TechType.UraniniteCrystal, TechType.MercuryOre };

            if (Main.config.Hardcore)
            {
                Dictionary<TechType, PDAScanner.EntryData> map = PDAScanner.mapping;

                foreach (TechType techType in Enum.GetValues(typeof(TechType)))
                {
                    if (!map.ContainsKey(techType) && !techType.ToString().Contains("Egg"))
                    {
#if SUBNAUTICA
                        if (CraftData.Get(techType, true) == null || techTypes.Contains(techType))
                        {
#elif BELOWZERO
                        if(TechData.GetIngredients(techType) == null || techType == TechType.Titanium)
                        {
#endif
                            PDAScanner.EntryData entryData = new PDAScanner.EntryData()
                            {
                                key = techType,
                                destroyAfterScan = false,
                                isFragment = false,
                                locked = true,
                                scanTime = 2f,
                                totalFragments = 1
                            };
                            map.Add(techType, entryData);
                        }
                    }
                }

            }
        }
    }

}