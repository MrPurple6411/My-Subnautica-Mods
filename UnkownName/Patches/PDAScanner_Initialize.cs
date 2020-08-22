using System;
using System.Collections.Generic;
using HarmonyLib;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using UnityEngine;
#if SUBNAUTICA
using Data = SMLHelper.V2.Crafting.TechData;
#elif BELOWZERO
using Data = SMLHelper.V2.Crafting.RecipeData;
#endif

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Initialize))]
    public class PDAScanner_Initialize
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (Main.config.Hardcore)
            {
                Dictionary<TechType, PDAScanner.EntryData> map = PDAScanner.mapping;
                List<TechType> techTypes = new List<TechType> 
                { 
                    TechType.Titanium, TechType.Copper, TechType.Quartz, TechType.Silver, TechType.Gold, 
                    TechType.Diamond, TechType.Lead, TechType.CreepvineSeedCluster, TechType.JellyPlant,
                    TechType.JeweledDiskPiece, TechType.GenericJeweledDisk, TechType.CreepvinePiece, TechType.AluminumOxide, TechType.Nickel, 
                    TechType.Kyanite, TechType.UraniniteCrystal, TechType.MercuryOre 
                };

                foreach (TechType techType in Enum.GetValues(typeof(TechType)))
                {
                    if (!map.ContainsKey(techType) || techTypes.Contains(techType))
                    {
                        Data data = Main.GetData(techType);
                        if (data == null || techTypes.Contains(techType))
                        {
                            map[techType] = new PDAScanner.EntryData()
                            {
                                key = techType,
                                blueprint = techType,
                                destroyAfterScan = false,
                                isFragment = false,
                                locked = true,
                                scanTime = 2f,
                                totalFragments = 1
                            };
                        }
                    }
                }
            }
        }
    }

}