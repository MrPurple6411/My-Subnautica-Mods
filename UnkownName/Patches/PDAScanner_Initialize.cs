using System;
using System.Collections.Generic;
using HarmonyLib;
using QModManager.Utility;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using UnityEngine;
using Logger = QModManager.Utility.Logger;
#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

#if SN1
using Data = SMLHelper.V2.Crafting.TechData;
#elif BZ
using Data = SMLHelper.V2.Crafting.RecipeData;
#endif

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Initialize))]
    public class PDAScanner_Initialize
    {
        [HarmonyPostfix]
        [HarmonyAfter(new string[] { "com.ahk1221.smlhelper" })]
        public static void Postfix()
        {
            if (Main.config.Hardcore)
            {
                Dictionary<TechType, PDAScanner.EntryData> map = PDAScanner.mapping;

                foreach (TechType techType in Enum.GetValues(typeof(TechType)))
                {
                    Data data = Main.GetData(techType);
                    map.TryGetValue(techType, out PDAScanner.EntryData entryData);

                    if (data is null && entryData != null && !entryData.isFragment && entryData.blueprint == TechType.None)
                    {
                        entryData.blueprint = techType;
                        entryData.locked = true;
                        continue;
                    }

                    if (data != null && entryData is null && (data.ingredientCount == 0 || techType == TechType.Titanium))
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
                        continue;
                    }

                    if (data != null && entryData !=  null && !entryData.isFragment && entryData.blueprint == TechType.None && (data.ingredientCount == 0 || techType == TechType.Titanium))
                    {
                        entryData.blueprint = techType;
                        entryData.locked = true;
                        continue;
                    }

                    if (data is null && entryData is null)
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
                        continue;
                    }
                }
            }
        }
    }

}