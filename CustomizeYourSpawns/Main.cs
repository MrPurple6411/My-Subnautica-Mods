using HarmonyLib;
#if SUBNAUTICA
using Oculus.Newtonsoft.Json;
using Oculus.Newtonsoft.Json.Converters;
#elif BELOWZERO
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
#endif
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UWE;
using static LootDistributionData;
using Logger = QModManager.Utility.Logger;

namespace CustomizeYourSpawns
{
    [QModCore]
    public static partial class Main
    {
        internal static string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static DirectoryInfo ChangesPath = Directory.CreateDirectory(ModPath+ "/ChangesToLoad");
        internal static string DefaultDistributions = ModPath + "/DefaultDistributions.json";
        internal static string BiomeDictionary = ModPath + "/BiomeList.json";
        internal static string ExampleFile = ModPath + "/ExampleFile.json";

        [QModPostPatch]
        public static void Load()
        {
            EnsureDefaultDistributions();
            EnsureBiomeDictionary();
            EnsureExample();

            LoadChangeFiles();
        }

        private static void EnsureDefaultDistributions()
        {
            if (!File.Exists(DefaultDistributions))
            {
                SortedDictionary<string, List<BiomeData>> defaultDistributions = new SortedDictionary<string, List<BiomeData>>();
                LootDistributionData data = LootDistributionData.Load("Balance/EntityDistributions");
                foreach (KeyValuePair<string, SrcData> pair in data.srcDistribution)
                {
                    if (WorldEntityDatabase.TryGetInfo(pair.Key, out WorldEntityInfo info))
                    {
                        if (info.techType != TechType.None)
                        {
                            defaultDistributions[info.techType.AsString()] = pair.Value.distribution;
                        }
                    }
                }

                using (StreamWriter writer = new StreamWriter(DefaultDistributions))
                {
                    writer.Write(JsonConvert.SerializeObject(defaultDistributions, Formatting.Indented, new StringEnumConverter() {
#if SUBNAUTICA
                        CamelCaseText = true,
#elif BELOWZERO
                        NamingStrategy = new CamelCaseNamingStrategy(), 
#endif
                        AllowIntegerValues = true })) ;
                }
            }
        }

        private static void EnsureBiomeDictionary()
        {
            if (!File.Exists(BiomeDictionary))
            {

                SortedDictionary<int, string> biomeDictionary = new SortedDictionary<int, string>();
                foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)))
                {
                    biomeDictionary[(int)biome] = biome.AsString();
                }

                using (StreamWriter writer = new StreamWriter(BiomeDictionary))
                {
                    writer.Write(JsonConvert.SerializeObject(biomeDictionary, Formatting.Indented));
                }
            }
        }

        private static void EnsureExample()
        {
            if (!File.Exists(ExampleFile))
            {
                Dictionary<TechType, List<BiomeData>> example = new Dictionary<TechType, List<BiomeData>>() {
                    {
                        TechType.GenericJeweledDisk,
                        new List<BiomeData>()
                        {
                            new BiomeData()
                            {
                                biome = BiomeType.SafeShallows_Wall,
                                count = 1,
                                probability = 1
                            }
                        }
                    },
                    {
                        TechType.AcidMushroom,
                        new List<BiomeData>()
                        {
                            new BiomeData()
                            {
                                biome = BiomeType.SafeShallows_Grass,
                                count = 4,
                                probability = 1
                            },
                            new BiomeData()
                            {
                                biome = BiomeType.SafeShallows_Plants,
                                count = 6,
                                probability = 1
                            }
                        }

                    }
                };
                using (StreamWriter writer = new StreamWriter(ExampleFile))
                {
                    writer.Write(JsonConvert.SerializeObject(example, Formatting.Indented, new StringEnumConverter()
                    {
#if SUBNAUTICA
                        CamelCaseText = true,
#elif BELOWZERO
                        NamingStrategy = new CamelCaseNamingStrategy(), 
#endif
                        AllowIntegerValues = true }));
                }
            }
        }

        private static void LoadChangeFiles()
        {
            Dictionary<TechType, List<BiomeData>> modifiedDistributions = new Dictionary<TechType, List<BiomeData>>();
            foreach (FileInfo file in ChangesPath.GetFiles().Where((x) => x.Extension.ToLower() == ".json"))
            {
                try
                {
                    if (!file.Name.ToLower().Contains("disabled"))
                    {
                        SortedDictionary<string, List<BiomeData>> pairs;
                        using (StreamReader reader = new StreamReader(file.FullName))
                        {
                            pairs = JsonConvert.DeserializeObject<SortedDictionary<string, List<BiomeData>>>(reader.ReadToEnd(), new StringEnumConverter()
                            {
#if SUBNAUTICA
                                CamelCaseText = true,
#elif BELOWZERO
                                NamingStrategy = new CamelCaseNamingStrategy(), 
#endif
                                AllowIntegerValues = true }) ?? new SortedDictionary<string, List<BiomeData>>();
                        }
                        int Succeded = 0;
                        foreach (KeyValuePair<string, List<BiomeData>> pair in pairs)
                        {
                            if (TechTypeExtensions.FromString(pair.Key, out TechType techType, true))
                            {
                                if (modifiedDistributions.ContainsKey(techType))
                                {
                                    List<BiomeData> datas = modifiedDistributions[techType];
                                    pair.Value.ForEach((x) => datas.Add(x));
                                    datas.Distinct();
                                    Succeded++;
                                }
                                else
                                {
                                    modifiedDistributions[techType] = pair.Value;
                                    Succeded++;
                                }
                            }
                            else
                            {
                                Logger.Log(Logger.Level.Debug, $"TechType: {pair.Key} not found --- do you have its mod installed?");
                            }
                        }
                        Logger.Log(Logger.Level.Debug, $"Successfully loaded file: {file.Name} with {Succeded} TechTypes being altered.");
                    }
                    else
                    {
                        Logger.Log(Logger.Level.Debug, $"Ignored file: {file.Name} as it contains the keyword 'disabled' in the name.");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(Logger.Level.Error, $"Failed to load {file.Name}.", e);
                }

            }

            if (modifiedDistributions.Count > 0)
            {
                RegisterChanges(modifiedDistributions);
            }
        }

        private static void RegisterChanges(Dictionary<TechType, List<BiomeData>> modifiedDistributions)
        {
            foreach (KeyValuePair<TechType, List<BiomeData>> pair in modifiedDistributions)
            {
                string classId = CraftData.GetClassIdForTechType(pair.Key) ?? pair.Key.AsString();
                if (PrefabDatabase.TryGetPrefabFilename(classId, out string prefabPath))
                {
                    if (!WorldEntityDatabase.TryGetInfo(classId, out WorldEntityInfo info))
                    {
                        info = new WorldEntityInfo()
                        {
                            cellLevel = LargeWorldEntity.CellLevel.Medium,
                            classId = classId,
                            localScale = UnityEngine.Vector3.one,
                            prefabZUp = false,
                            slotType = EntitySlot.Type.Medium,
                            techType = pair.Key
                        };

                        WorldEntityDatabaseHandler.AddCustomInfo(classId, info);
                    }

                    SrcData data = new SrcData() { prefabPath = prefabPath, distribution = pair.Value };
                    Logger.Log(Logger.Level.Debug, $"Altering Spawn Locations for {pair.Key.AsString()}. Adding {data.distribution.Count} values");
                    LootDistributionHandler.AddLootDistributionData(classId, data);
                }
                else
                {
                    Logger.Log(Logger.Level.Warn, $"Failed to get PrefabPath for {pair.Key.AsString()}. Skipped editing distribution for {pair.Key.AsString()}");
                }
            }
        }

    }
}