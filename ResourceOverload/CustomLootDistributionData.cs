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
        public static List<TechType> randomizedResources = new List<TechType>() 
        { 
            TechType.AluminumOxide, TechType.Sulphur, TechType.Diamond, 
            TechType.Kyanite, TechType.Lead, TechType.Lithium , TechType.Magnetite, 
            TechType.Nickel, TechType.Quartz, TechType.Silver, TechType.UraniniteCrystal, 
            TechType.Salt, TechType.AcidMushroom, TechType.BloodOil, TechType.JellyPlant, 
            TechType.DrillableAluminiumOxide, TechType.DrillableCopper, TechType.DrillableDiamond,
            TechType.DrillableGold, TechType.DrillableKyanite, TechType.DrillableLead,
            TechType.DrillableLithium, TechType.DrillableMagnetite, TechType.DrillableMercury,
            TechType.DrillableNickel, TechType.DrillableQuartz, TechType.DrillableSalt,
            TechType.DrillableSilver, TechType.DrillableSulphur, TechType.DrillableTitanium,
            TechType.DrillableUranium, TechType.TimeCapsule, TechType.Titanium, 
            TechType.Copper, TechType.Gold
        };
        public static List<TechType> randomizedChunks = new List<TechType>()
        {
            TechType.LimestoneChunk, TechType.SandstoneChunk, TechType.ShaleChunk
        };
        public static List<TechType> randomizedFragments = new List<TechType>();

        [HarmonyPrefix]
        public static void Postfix(LootDistributionData __instance, BiomeType biome, ref bool __result, ref LootDistributionData.DstData data)
        {
            if (customDSTDistribution.Count == 0 || Config.RegenSpawns)
                GenerateCustomData(__instance);

            if (__result)
                __result = customDSTDistribution.TryGetValue(biome, out data);
        }

        private static void GenerateCustomData(LootDistributionData __instance)
        {
            Config.techProbability = new SortedList<string, float>();
            customDSTDistribution = new SortedDictionary<BiomeType, LootDistributionData.DstData>();
            foreach (BiomeType bio in Enum.GetValues(typeof(BiomeType)))
            {
                techs = new SortedDictionary<TechType, LootDistributionData.PrefabData>();
                if (__instance.dstDistribution.ContainsKey(bio))
                {
                    customDSTDistribution[bio] = new LootDistributionData.DstData() { prefabs = new List<LootDistributionData.PrefabData>() };

                    CheckSettings(bio);
                    if (Config.Randomization)
                        Randomizer(__instance, bio);
                    LoadOriginalDistribution(__instance, bio);
                }
                GenerateMissingConfiguration(bio);
            }
            if (Config.RegenSpawns || Config.resetDefaults)
            {
                Config.resetDefaults = false;
                Config.RegenSpawns = false;
            }
        }

        #region Settings
        private static void CheckSettings(BiomeType bio)
        {
            if (Config.resetDefaults)
            {
                ResetSettings(bio);
            }
            else
            {
                LoadSettings(bio);
            }
        }

        private static void LoadSettings(BiomeType bio)
        {
            foreach (TechType type in Enum.GetValues(typeof(TechType)))
            {
                string tech0;
                if (Config.Randomization)
                {
                    tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type);
                }
                else
                {
                    tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type) + "| " + bio.AsString().Split('_')[0];
                }
                if (PlayerPrefs.HasKey(tech0 + ":TechProbability"))
                {
                    Config.techProbability[tech0] = PlayerPrefs.GetFloat(tech0 + ":TechProbability");
                }
            }
        }

        private static void ResetSettings(BiomeType bio)
        {
            foreach (TechType type in Enum.GetValues(typeof(TechType)))
            {
                string tech0;
                tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type);
                if (PlayerPrefs.HasKey(tech0 + ":TechProbability"))
                {
                    PlayerPrefs.DeleteKey(tech0 + ":TechProbability");
                }

                tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type) + "| " + bio.AsString().Split('_')[0];
                if (PlayerPrefs.HasKey(tech0 + ":TechProbability"))
                {
                    PlayerPrefs.DeleteKey(tech0 + ":TechProbability");
                }
            }
        }

        private static void GenerateMissingConfiguration(BiomeType biomeType)
        {
            foreach (TechType type in techs.Keys)
            {
                string tech0;
                if (Config.Randomization)
                {
                    tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type);
                }
                else
                {
                    tech0 = TechTypeExtensions.GetOrFallback(Language.main, type, type) + "| " + biomeType.AsString().Split('_')[0];
                }
                if (!Config.techProbability.ContainsKey(tech0))
                {
                    if (type == TechType.TimeCapsule)
                    {
                        Config.techProbability[tech0] = techs[type].probability * 1000;
                    }
                    else
                    {
                        Config.techProbability[tech0] = techs[type].probability * 100;
                    }
                }
            }
        }

        #endregion

        private static void Randomizer(LootDistributionData __instance, BiomeType bio)
        {
            foreach (BiomeType b in Enum.GetValues(typeof(BiomeType)))
            {
                if (!bio.AsString().Contains("Fragment") && __instance.dstDistribution.TryGetValue(b, out var d))
                {
                    for (int i = 0; i < d.prefabs.Count; i++)
                    {
                        LootDistributionData.PrefabData prefabData = d.prefabs[i];
                        if (prefabData.classId.ToLower() != "none")
                        {
                            if (WorldEntityDatabase.TryGetInfo(prefabData.classId, out WorldEntityInfo wei))
                            {
                                if (randomizedChunks.Contains(wei.techType) && CheckBiomeForTechTypes(__instance, bio, randomizedChunks))
                                {
                                    AddPrefabToCustomData(prefabData, bio, wei);
                                    continue;
                                }
                                if (randomizedResources.Contains(wei.techType) && CheckBiomeForTechTypes(__instance, bio, randomizedResources))
                                {
                                    AddPrefabToCustomData(prefabData, bio, wei);
                                    continue;
                                }
                                if(randomizedFragments.Count == 0)
                                {
                                    foreach(TechType techType in Enum.GetValues(typeof(TechType)))
                                    {
                                        if (techType.AsString().Contains("Fragment"))
                                            randomizedFragments.Add(techType);
                                    }
                                }
                                if (randomizedFragments.Contains(wei.techType) && CheckBiomeForTechTypes(__instance, bio, randomizedFragments))
                                {
                                    AddPrefabToCustomData(prefabData, bio, wei);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            for (int j = 0; j < customDSTDistribution[bio].prefabs.Count; j++)
            {
                LootDistributionData.PrefabData prefabData = customDSTDistribution[bio].prefabs[j];
                prefabData.probability = 0.1f;
            }
        }

        private static void AddPrefabToCustomData(LootDistributionData.PrefabData prefabData, BiomeType bio, WorldEntityInfo wei)
        {
            if (!techs.ContainsKey(wei.techType))
            {
                if (Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]))
                {
                    if (wei.techType == TechType.TimeCapsule)
                    {
                        prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 1000;
                    }
                    else
                    {
                        prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + bio.AsString().Split('_')[0]] / 100;
                    }
                    customDSTDistribution[bio].prefabs.Add(prefabData);
                }
                else if(Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType)))
                {
                    if (wei.techType == TechType.TimeCapsule)
                    {
                        prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType)] / 1000;
                    }
                    else
                    {
                        prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType)] / 100;
                    }
                    customDSTDistribution[bio].prefabs.Add(prefabData);
                }
                else
                {
                    techs[wei.techType] = prefabData;
                    customDSTDistribution[bio].prefabs.Add(prefabData);
                }
            }
            else if (prefabData.probability > 0 && (prefabData.probability < techs[wei.techType].probability || techs[wei.techType].probability == 0 || techs[wei.techType].probability == 1))
            {
                techs[wei.techType] = prefabData;
                customDSTDistribution[bio].prefabs.Add(prefabData);
            }
        }

        private static bool CheckBiomeForTechTypes(LootDistributionData lootDistributionData, BiomeType biomeType, List<TechType> techTypes)
        {
            foreach (LootDistributionData.PrefabData prefab in lootDistributionData.dstDistribution[biomeType].prefabs)
            {
                if (WorldEntityDatabase.TryGetInfo(prefab.classId, out WorldEntityInfo wei2))
                {
                    if (techTypes.Contains(wei2.techType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void LoadOriginalDistribution(LootDistributionData lootDistributionData, BiomeType biomeType)
        {
            foreach (LootDistributionData.PrefabData prefabData in lootDistributionData.dstDistribution[biomeType].prefabs)
            {
                if (WorldEntityDatabase.TryGetInfo(prefabData.classId, out WorldEntityInfo wei))
                {
                    if (wei.techType != TechType.None)
                    {
                        if (!Config.techProbability.ContainsKey(TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + biomeType.AsString().Split('_')[0]))
                        {
                            techs[wei.techType] = prefabData;
                            customDSTDistribution[biomeType].prefabs.Add(prefabData);
                            continue;
                        }
                        else
                        {
                            prefabData.probability = Config.techProbability[TechTypeExtensions.GetOrFallback(Language.main, wei.techType, wei.techType) + "| " + biomeType.AsString().Split('_')[0]] / 100;
                            customDSTDistribution[biomeType].prefabs.Add(prefabData);
                            continue;
                        }
                    }
                    else
                    {
                        customDSTDistribution[biomeType].prefabs.Add(prefabData);
                        continue;
                    }
                }
                else
                {
                    customDSTDistribution[biomeType].prefabs.Add(prefabData);
                }
            }
        }

    }
}
