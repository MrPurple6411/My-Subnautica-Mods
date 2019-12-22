using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;
using UWE;

namespace ResourceOverload
{
    [HarmonyPatch(typeof(LootDistributionData))]
    [HarmonyPatch("GetBiomeLoot")]
    public static class CustomLootDistributionData
    {
        public static SortedDictionary<TechType, LootDistributionData.PrefabData> techs;
        public static SortedDictionary<BiomeType, LootDistributionData.DstData> customDSTDistribution = new SortedDictionary<BiomeType, LootDistributionData.DstData>();

        [HarmonyPrefix]
        public static bool Prefix(LootDistributionData __instance, BiomeType biome, ref bool __result, out LootDistributionData.DstData data)
        {
            if (customDSTDistribution.Count == 0 || Config.RegenSpawns)
            {
                Config.techProbability = new SortedList<string, float>();
                customDSTDistribution = new SortedDictionary<BiomeType, LootDistributionData.DstData>();
                foreach (BiomeType bio in Enum.GetValues(typeof(BiomeType)))
                {
                    string x = bio.AsString().Split('_').GetLast<string>();
                    techs = new SortedDictionary<TechType, LootDistributionData.PrefabData>();
                    if (__instance.dstDistribution.ContainsKey(bio))
                    {
                        if (!Config.resetDefaults)
                        {
                            foreach (TechType type in Enum.GetValues(typeof(TechType)))
                            {
                                string tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type) + "| " + bio.AsString().Split('_')[0];
                                if (PlayerPrefs.HasKey(tech0 + ":TechProbability"))
                                {
                                    Config.techProbability[tech0] = PlayerPrefs.GetFloat(tech0 + ":TechProbability");
                                }
                            }
                        }
                        else
                        {
                            foreach (TechType type in Enum.GetValues(typeof(TechType)))
                            {
                                    string tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type) + "| " + bio.AsString().Split('_')[0];
                                    if (PlayerPrefs.HasKey(tech0 + ":TechProbability"))
                                    {
                                        PlayerPrefs.DeleteKey(tech0 + ":TechProbability");
                                    }
                            }
                        }

                        customDSTDistribution[bio] = new LootDistributionData.DstData
                        {
                            prefabs = new List<LootDistributionData.PrefabData>()
                        };

                        foreach (BiomeType b in Enum.GetValues(typeof(BiomeType)))
                        {
                            if (Config.Randomization && __instance.dstDistribution.TryGetValue(b, out var d))
                            {
                                foreach (LootDistributionData.PrefabData prefabData in d.prefabs)
                                {
                                    if (prefabData.classId.ToLower() != "none")
                                    {
                                        if (WorldEntityDatabase.TryGetInfo(prefabData.classId, out WorldEntityInfo wei))
                                        {
                                            if (!bio.AsString().Contains("Fragment") && wei.slotType == EntitySlot.Type.Creature)
                                            {
                                                if (wei.techType == TechType.ReaperLeviathan)
                                                {
                                                    bool check = false;
                                                    foreach (LootDistributionData.PrefabData prefab in __instance.dstDistribution[bio].prefabs)
                                                    {
                                                        if (WorldEntityDatabase.TryGetInfo(prefab.classId, out WorldEntityInfo wei2))
                                                        {
                                                            if (wei2.techType == TechType.Sandshark ||
                                                                wei2.techType == TechType.BoneShark ||
                                                                wei2.techType == TechType.SpineEel ||
                                                                wei2.techType == TechType.Shocker ||
                                                                wei2.techType == TechType.CrabSquid ||
                                                                wei2.techType == TechType.LavaLizard ||
                                                                wei2.techType == TechType.WarperSpawner)
                                                            {
                                                                check = true;
                                                            }
                                                        }
                                                    }
                                                    if (check)
                                                    {
                                                        if (!techs.ContainsKey(wei.techType))
                                                        {
                                                            if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                            {
                                                                prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 1000;
                                                                customDSTDistribution[bio].prefabs.Add(prefabData);
                                                                continue;
                                                            }
                                                            else
                                                            {
                                                                prefabData.probability = 0.001f;
                                                                Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] = prefabData.probability * 1000;
                                                                customDSTDistribution[bio].prefabs.Add(prefabData);
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (wei.techType == TechType.Shocker || wei.techType == TechType.CrabSquid || wei.techType == TechType.WarperSpawner)
                                                {
                                                    bool check = false;
                                                    foreach (LootDistributionData.PrefabData prefab in __instance.dstDistribution[bio].prefabs)
                                                    {
                                                        if (WorldEntityDatabase.TryGetInfo(prefab.classId, out WorldEntityInfo wei2))
                                                        {
                                                            if (wei2.techType == TechType.Sandshark ||
                                                                wei2.techType == TechType.BoneShark ||
                                                                wei2.techType == TechType.SpineEel ||
                                                                wei2.techType == TechType.Shocker ||
                                                                wei2.techType == TechType.CrabSquid ||
                                                                wei2.techType == TechType.Crabsnake ||
                                                                wei2.techType == TechType.LavaLizard)
                                                            {
                                                                check = true;
                                                            }
                                                        }
                                                    }
                                                    if (check)
                                                    {
                                                        if (!techs.ContainsKey(wei.techType))
                                                        {
                                                            if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                            {
                                                                prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 100;
                                                                customDSTDistribution[bio].prefabs.Add(prefabData);
                                                                continue;
                                                            }
                                                            else
                                                            {
                                                                techs[wei.techType] = prefabData;
                                                                customDSTDistribution[bio].prefabs.Add(prefabData);
                                                                continue;
                                                            }
                                                        }
                                                        else if (prefabData.probability > 0 && (prefabData.probability < techs[wei.techType].probability || techs[wei.techType].probability == 0) && !Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                        {
                                                            techs[wei.techType] = prefabData;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                    }
                                                }
                                                if (wei.techType == TechType.Mesmer)
                                                {
                                                    bool check = false;
                                                    foreach (LootDistributionData.PrefabData prefab in __instance.dstDistribution[bio].prefabs)
                                                    {
                                                        if (WorldEntityDatabase.TryGetInfo(prefabData.classId, out WorldEntityInfo wei2))
                                                        {
                                                            if (BehaviourData.GetBehaviourType(wei2.techType) == BehaviourType.SmallFish ||
                                                                wei2.techType == TechType.Peeper ||
                                                                wei2.techType == TechType.Mesmer)
                                                            {
                                                                check = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (check)
                                                    {
                                                        if (!techs.ContainsKey(wei.techType))
                                                        {
                                                            if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                            {
                                                                prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 100;
                                                                customDSTDistribution[bio].prefabs.Add(prefabData);
                                                                continue;
                                                            }
                                                            else
                                                            {
                                                                techs[wei.techType] = prefabData;
                                                                customDSTDistribution[bio].prefabs.Add(prefabData);
                                                                continue;
                                                            }
                                                        }
                                                        else if (prefabData.probability > 0 && (prefabData.probability < techs[wei.techType].probability || techs[wei.techType].probability == 0) && !Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                        {
                                                            techs[wei.techType] = prefabData;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                    }
                                                }
                                            }
                                            else if (wei.techType != TechType.None && !bio.AsString().Contains("Fragment") && wei.techType.AsString().Contains("Chunk"))
                                            {
                                                bool check = false;
                                                foreach (LootDistributionData.PrefabData prefab in __instance.dstDistribution[bio].prefabs)
                                                {
                                                    if (WorldEntityDatabase.TryGetInfo(prefab.classId, out WorldEntityInfo wei2))
                                                    {
                                                        if (wei2.techType == TechType.LimestoneChunk ||
                                                            wei2.techType == TechType.SandstoneChunk ||
                                                            wei2.techType == TechType.ShaleChunk)
                                                        {
                                                            check = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (check)
                                                {
                                                    if (!techs.ContainsKey(wei.techType))
                                                    {
                                                        if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                        {
                                                            prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 100;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            techs[wei.techType] = prefabData;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                    }
                                                    else if (prefabData.probability > 0 && (prefabData.probability < techs[wei.techType].probability || techs[wei.techType].probability == 0) && !Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                    {
                                                        techs[wei.techType] = prefabData;
                                                        customDSTDistribution[bio].prefabs.Add(prefabData);
                                                        continue;
                                                    }
                                                }
                                            }
                                            else if (wei.techType != TechType.None && !bio.AsString().Contains("Fragment") && (wei.techType == TechType.AluminumOxide ||
                                                wei.techType == TechType.BloodOil ||
                                                wei.techType == TechType.Sulphur ||
                                                wei.techType == TechType.Diamond ||
                                                wei.techType == TechType.Kyanite ||
                                                wei.techType == TechType.Lead ||
                                                wei.techType == TechType.Lithium ||
                                                wei.techType == TechType.Magnetite ||
                                                wei.techType == TechType.Nickel ||
                                                wei.techType == TechType.Quartz ||
                                                wei.techType == TechType.Silver))
                                            {
                                                bool check = false;
                                                foreach (LootDistributionData.PrefabData prefab in __instance.dstDistribution[bio].prefabs)
                                                {
                                                    if (WorldEntityDatabase.TryGetInfo(prefab.classId, out WorldEntityInfo wei2))
                                                    {
                                                        if (wei2.techType == TechType.AluminumOxide ||
                                                            wei2.techType == TechType.BloodOil ||
                                                            wei2.techType == TechType.Sulphur ||
                                                            wei2.techType == TechType.Diamond ||
                                                            wei2.techType == TechType.Kyanite ||
                                                            wei2.techType == TechType.Lead ||
                                                            wei2.techType == TechType.Lithium ||
                                                            wei2.techType == TechType.Magnetite ||
                                                            wei2.techType == TechType.Nickel ||
                                                            wei2.techType == TechType.Salt ||
                                                            wei2.techType == TechType.Silver)
                                                        {
                                                            check = true;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (check)
                                                {
                                                    if (!techs.ContainsKey(wei.techType))
                                                    {
                                                        if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                        {
                                                            prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 100;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            techs[wei.techType] = prefabData;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                    }
                                                    else if (prefabData.probability > 0 && (prefabData.probability < techs[wei.techType].probability || techs[wei.techType].probability == 0) && !Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                    {
                                                        techs[wei.techType] = prefabData;
                                                        customDSTDistribution[bio].prefabs.Add(prefabData);
                                                        continue;
                                                    }
                                                }
                                            }
                                            else if (wei.techType != TechType.None && !bio.AsString().Contains("Fragment") && wei.techType.AsString().Contains("Drillable"))
                                            {
                                                bool check = false;
                                                foreach (LootDistributionData.PrefabData prefab in __instance.dstDistribution[bio].prefabs)
                                                {
                                                    if (WorldEntityDatabase.TryGetInfo(prefab.classId, out WorldEntityInfo wei2))
                                                    {
                                                        if (wei2.techType.AsString().Contains("Drillable"))
                                                        {
                                                            check = true;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (check)
                                                {
                                                    if (!techs.ContainsKey(wei.techType))
                                                    {
                                                        if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                        {
                                                            prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 100;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            techs[wei.techType] = prefabData;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                    }
                                                    else if (prefabData.probability > 0 && (prefabData.probability < techs[wei.techType].probability || techs[wei.techType].probability == 0) && !Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                    {
                                                        techs[wei.techType] = prefabData;
                                                        customDSTDistribution[bio].prefabs.Add(prefabData);
                                                        continue;
                                                    }
                                                }
                                            }
                                            else if (((!bio.AsString().Contains("Fragment") &&
                                                !b.AsString().Contains("Fragment")) || (bio.AsString().Contains("Fragment") &&
                                                b.AsString().Contains("Fragment"))) &&
                                                wei.techType.AsString().Contains("Fragment"))
                                            {
                                                bool check = false;
                                                foreach (LootDistributionData.PrefabData prefab in __instance.dstDistribution[bio].prefabs)
                                                {
                                                    if (WorldEntityDatabase.TryGetInfo(prefab.classId, out WorldEntityInfo wei2))
                                                    {
                                                        if (wei2.techType.AsString().Contains("Fragment"))
                                                        {
                                                            check = true;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (check)
                                                {
                                                    if (!techs.ContainsKey(wei.techType))
                                                    {
                                                        if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                        {
                                                            prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 100;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                        else if(prefabData.probability > 0)
                                                        {
                                                            techs[wei.techType] = prefabData;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                    }
                                                    else if (prefabData.probability > 0 && (prefabData.probability < techs[wei.techType].probability || techs[wei.techType].probability == 0) && !Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                    {
                                                        techs[wei.techType] = prefabData;
                                                        customDSTDistribution[bio].prefabs.Add(prefabData);
                                                        continue;
                                                    }
                                                }
                                            }
                                            else if (!bio.AsString().Contains("Fragment") && wei.techType == TechType.TimeCapsule)
                                            {
                                                bool check = false;
                                                foreach (LootDistributionData.PrefabData prefab in __instance.dstDistribution[b].prefabs)
                                                {
                                                    if (WorldEntityDatabase.TryGetInfo(prefab.classId, out WorldEntityInfo wei2))
                                                    {
                                                        if (wei2.techType.AsString().Contains("Fragment"))
                                                        {
                                                            check = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (check)
                                                {
                                                    if (!techs.ContainsKey(wei.techType))
                                                    {
                                                        if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                        {
                                                            prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 1000;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            techs[wei.techType] = prefabData;
                                                            customDSTDistribution[bio].prefabs.Add(prefabData);
                                                            continue;
                                                        }
                                                    }
                                                    else if(prefabData.probability > 0 && (prefabData.probability < techs[wei.techType].probability || techs[wei.techType].probability == 0) && !Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                                    {
                                                        techs[wei.techType] = prefabData;
                                                        customDSTDistribution[bio].prefabs.Add(prefabData);
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        foreach (LootDistributionData.PrefabData prefabData in __instance.dstDistribution[bio].prefabs)
                        {
                            if (WorldEntityDatabase.TryGetInfo(prefabData.classId, out WorldEntityInfo wei))
                            {
                                if (wei.techType != TechType.None)
                                {
                                    if (!Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                                    {
                                        techs[wei.techType] = prefabData;
                                        customDSTDistribution[bio].prefabs.Add(prefabData);
                                        continue;
                                    }
                                    else
                                    {
                                        prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 100;
                                        customDSTDistribution[bio].prefabs.Add(prefabData);
                                        continue;
                                    }
                                }
                                else
                                {
                                    customDSTDistribution[bio].prefabs.Add(prefabData);
                                    continue;
                                }
                            }
                            else
                            {
                                customDSTDistribution[bio].prefabs.Add(prefabData);
                            }
                        }
                    }
                    foreach (TechType type in techs.Keys)
                    {
                        string tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type) + "| " + bio.AsString().Split('_')[0];
                        if (!Config.techProbability.ContainsKey(tech0))
                        {
                            if (type == TechType.TimeCapsule)
                            {
                                Config.techProbability[tech0] = techs[type].probability * 1000;
                            }
                            else
                            {
                                if (techs[type].probability >= 0.1)
                                {
                                    techs[type].probability = techs[type].probability / 4;
                                }
                                Config.techProbability[tech0] = techs[type].probability * 100;
                            }
                        }
                    }
                }
                if (Config.RegenSpawns)
                {
                    Config.resetDefaults = false;
                    Config.RegenSpawns = false;
                    IngameMenu.main.Open();
                    IngameMenu.main.ChangeSubscreen("Options");
                }

            }

            __result = customDSTDistribution.TryGetValue(biome, out data);
            return !(__result);
        }
    }

    /*
    [HarmonyPatch(typeof(LootDistributionData))]
    [HarmonyPatch("GetBiomeLoot")]
    class AllBiomesLoot
    {
        public static LootDistributionData.DstData datax;

        public static void Prefix(LootDistributionData __instance, BiomeType biome)
        {
            if (datax == null)
            {
                Console.WriteLine("************************Purple Start************************");
                datax = new LootDistributionData.DstData();
                datax.prefabs = new System.Collections.Generic.List<LootDistributionData.PrefabData>();
                foreach (TechType tech in Enum.GetValues(typeof(TechType)))
                {
                    Console.WriteLine("\n************************" + tech.AsString() + "************************");
                    foreach (BiomeType b in Enum.GetValues(typeof(BiomeType)))
                    {
                        if (__instance.dstDistribution.TryGetValue(b, out var d))
                        {
                            foreach (LootDistributionData.PrefabData prefabData in d.prefabs)
                            {
                                WorldEntityInfo worldEntityInfo;
                                if (WorldEntityDatabase.TryGetInfo(prefabData.classId, out worldEntityInfo))
                                {
                                    if (worldEntityInfo.techType == tech)
                                    {
                                        Console.WriteLine("\nTechType: " + tech.AsString());
                                        Console.WriteLine("Biome: " + b.AsString());
                                        Console.WriteLine("prefabData.count: " + prefabData.count +
                                            "\nprefabData.probability: " + prefabData.probability);
                                        Console.WriteLine("worldEntityInfo.classId: " + worldEntityInfo.classId +
                                                "\nworldEntityInfo.cellLevel: " + worldEntityInfo.cellLevel +
                                                "\nworldEntityInfo.slotType: " + worldEntityInfo.slotType +
                                                "\nworldEntityInfo.localScale: " + worldEntityInfo.localScale);
                                    }
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("************************Purple End************************");
            }
        }
    }*/

}
