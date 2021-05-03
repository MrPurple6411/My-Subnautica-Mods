namespace CustomizeYourSpawns
{
    using HarmonyLib;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
    using Oculus.Newtonsoft.Json.Converters;
#elif BZ || SUBNAUTICA_EXP
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
#endif
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using UWE;
    using static LootDistributionData;
    using Logger = QModManager.Utility.Logger;

    [QModCore]
    public static partial class Main
    {
        internal static string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static DirectoryInfo ChangesPath = Directory.CreateDirectory(ModPath + "/ChangesToLoad");
        internal static string DefaultDistributions = ModPath + "/DefaultDistributions.json";
        internal static string BiomeDictionary = ModPath + "/BiomeList.json";
        internal static string ExampleFile = ModPath + "/ExampleFile.json";
        internal static string Names = ModPath + "/Names.json";
        private static readonly Dictionary<string, string> classIdName = new Dictionary<string, string>();
        private static Dictionary<string, TechType> nameTechType = new Dictionary<string, TechType>();
        private static Dictionary<TechType, List<string>> techTypeName = new Dictionary<TechType, List<string>>();
        private static Dictionary<string, List<string>> nameClassIds = new Dictionary<string, List<string>>();
        private static Dictionary<string, string> classIdPrefab = new Dictionary<string, string>();

        [QModPostPatch]
        public static void Load()
        {
            Setup();
            EnsureDefaultDistributions();
            EnsureBiomeDictionary();
            EnsureExample();
            LoadChangeFiles();
        }

        private static void Setup()
        {
            PrefabDatabase.LoadPrefabDatabase(SNUtils.prefabDatabaseFilename);
            HashSet<string> names = new HashSet<string>();

            foreach(KeyValuePair<string, string> prefabFile in PrefabDatabase.prefabFiles)
            {
                TechType techType = CraftData.GetTechForEntNameExpensive(Path.GetFileName(prefabFile.Value).Replace(".prefab", ""));
                string name = techType.AsString();
                //techType = TechType.Seamoth
                nameTechType[name] = techType;
                if(!techTypeName.TryGetValue(techType, out List<string> techNames))
                    techNames = new List<string>();
                if(!techNames.Contains(name))
                    techNames.Add(name);

                if(techType == TechType.None)
                {
                    name = prefabFile.Value;
                    name = name.Substring(name.LastIndexOf("/") + 1).Replace(".prefab", "");
                    //Logger.Log(Logger.Level.Debug, "TechType.None " + name);
                }

                names.Add(name);
                if(!nameClassIds.ContainsKey(name))
                    nameClassIds[name] = new List<string> { prefabFile.Key };
                else
                    nameClassIds[name].Add(prefabFile.Key);

                classIdName[prefabFile.Key] = name;
                classIdPrefab[prefabFile.Key] = prefabFile.Value;
                // Logger.Log(Logger.Level.Info, prefabFile.Key + " " + techType + " " + prefabFile.Value);
            }

            List<ModPrefab> modPreFabsList = Traverse.Create<ModPrefab>().Field<List<ModPrefab>>("PreFabsList").Value;

            foreach(ModPrefab modPrefab in modPreFabsList)
            {
                string name = modPrefab.TechType.AsString();
                TechType techType = modPrefab.TechType;

                nameTechType[name] = techType;
                if(!techTypeName.TryGetValue(techType, out List<string> techNames))
                    techNames = new List<string>();
                if(!techNames.Contains(name))
                    techNames.Add(name);

                names.Add(name);
                if(!nameClassIds.ContainsKey(name))
                    nameClassIds[name] = new List<string> { modPrefab.ClassID };
                else
                    nameClassIds[name].Add(modPrefab.ClassID);

                classIdName[modPrefab.ClassID] = name;
                classIdPrefab[modPrefab.ClassID] = modPrefab.PrefabFileName;

            }

            using(StreamWriter writer = new StreamWriter(Names))
                writer.Write(JsonConvert.SerializeObject(names, Formatting.Indented));
        }

        private static void EnsureDefaultDistributions()
        {
            if(File.Exists(DefaultDistributions))
                return;

            SortedDictionary<string, Dictionary<BiomeType, BiomeData>> defaultDistributions = new SortedDictionary<string, Dictionary<BiomeType, BiomeData>>();
            LootDistributionData lootDistData = LootDistributionData.Load("Balance/EntityDistributions");

            foreach(KeyValuePair<string, SrcData> srcDist in lootDistData.srcDistribution)
            {
                if(srcDist.Key != "None")
                {
                    string classId = srcDist.Key;
                    if(!classIdName.ContainsKey(classId))
                    {
                        Logger.Log(Logger.Level.Warn, "classIdName has no " + classId);
                    }
                    else
                    {
                        string name = classIdName[classId];
                        //string techType1 = "";
                        //if (nameTechType.ContainsKey(name))
                        //    techType1 = nameTechType[name].AsString();

                        //Logger.Log(Logger.Level.Info, "lootDistData " + techType1 + " " + classId + " " + srcDist.Value.prefabPath);
                        //foreach (BiomeData biomeData in srcDist.Value.distribution)
                        //    Logger.Log(Logger.Level.Info, biomeData.biome + " " + biomeData.count + " " + biomeData.probability);

                        if(!defaultDistributions.ContainsKey(name))
                        {
                            Dictionary<BiomeType, BiomeData> biomeDict = new Dictionary<BiomeType, BiomeData>();
                            foreach(BiomeData biomeData in srcDist.Value.distribution)
                                biomeDict[biomeData.biome] = biomeData;

                            defaultDistributions[name] = new Dictionary<BiomeType, BiomeData>(biomeDict);
                        }
                        else
                        {
                            foreach(BiomeData srcBiomeData in srcDist.Value.distribution)
                            {
                                if(!defaultDistributions[name].ContainsKey(srcBiomeData.biome))
                                {
                                    defaultDistributions[name][srcBiomeData.biome] = srcBiomeData;
                                }
                                else
                                {
                                    BiomeData savedBiomeData = defaultDistributions[name][srcBiomeData.biome];
                                    //Logger.Log(Logger.Level.Info, techType + " same biome " + srcBiomeData.biome);
                                    BiomeData newBiomeData = new BiomeData()
                                    {
                                        biome = srcBiomeData.biome,
                                        count = srcBiomeData.count + savedBiomeData.count,
                                        probability = (srcBiomeData.probability + savedBiomeData.probability) * 0.5f
                                    };
                                    defaultDistributions[name][srcBiomeData.biome] = newBiomeData;
                                }
                            }
                        }
                    }
                }
            }
            Logger.Log(Logger.Level.Info, "defaultDistributions count  " + defaultDistributions.Count);

            SortedDictionary<string, List<BiomeData>> defaultDistributionsL = new SortedDictionary<string, List<BiomeData>>();

            foreach(KeyValuePair<string, Dictionary<BiomeType, BiomeData>> defaultDistribution in defaultDistributions)
            {
                List<BiomeData> biomeDataList = new List<BiomeData>();
                foreach(KeyValuePair<BiomeType, BiomeData> biomeData in defaultDistribution.Value)
                    biomeDataList.Add(biomeData.Value);

                defaultDistributionsL[defaultDistribution.Key] = new List<BiomeData>(biomeDataList);
            }

            using(StreamWriter writer = new StreamWriter(DefaultDistributions))
            {
                writer.Write(JsonConvert.SerializeObject(defaultDistributionsL, Formatting.Indented, new JsonConverter[] {

                    new StringEnumConverter() {
#if SUBNAUTICA_STABLE
                    CamelCaseText = true,
#else
                    NamingStrategy = new CamelCaseNamingStrategy(), 
#endif
                    AllowIntegerValues = true },

                    new TechTypeConverter()
                }));
            }
        }

        private static void EnsureBiomeDictionary()
        {
            if(!File.Exists(BiomeDictionary))
            {
                SortedDictionary<int, string> biomeDictionary = new SortedDictionary<int, string>();
                foreach(BiomeType biome in Enum.GetValues(typeof(BiomeType)))
                    biomeDictionary[(int)biome] = biome.AsString();

                using(StreamWriter writer = new StreamWriter(BiomeDictionary))
                    writer.Write(JsonConvert.SerializeObject(biomeDictionary, Formatting.Indented));
            }
        }

        private static void EnsureExample()
        {
            if(!File.Exists(ExampleFile))
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
                        TechType.Tank,
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
                using(StreamWriter writer = new StreamWriter(ExampleFile))
                {
                    writer.Write(JsonConvert.SerializeObject(example, Formatting.Indented, new StringEnumConverter()
                    {
#if SUBNAUTICA_STABLE
                        CamelCaseText = true,
#else
                        NamingStrategy = new CamelCaseNamingStrategy(),
#endif
                        AllowIntegerValues = true
                    }));
                }
            }
        }

        private static void LoadChangeFiles()
        {
            Dictionary<string, List<BiomeData>> modifiedDistributions = new Dictionary<string, List<BiomeData>>();
            foreach(FileInfo file in ChangesPath.GetFiles().Where((x) => x.Extension.ToLower() == ".json"))
            {
                try
                {
                    if(!file.Name.ToLower().Contains("disabled"))
                    {
                        SortedDictionary<string, List<BiomeData>> nameBiomeList;
                        using(StreamReader reader = new StreamReader(file.FullName))
                        {
                            nameBiomeList = JsonConvert.DeserializeObject<SortedDictionary<string, List<BiomeData>>>(reader.ReadToEnd(), new JsonConverter[] {new StringEnumConverter()
                            {
#if SUBNAUTICA_STABLE
                                CamelCaseText = true,
#else
                                NamingStrategy = new CamelCaseNamingStrategy(), 
#endif
                                AllowIntegerValues = true
                            },
                                new TechTypeConverter()
                            }) ?? new SortedDictionary<string, List<BiomeData>>();
                        }
                        int succeded = 0;

                        foreach(KeyValuePair<string, List<BiomeData>> nameBiomeData in nameBiomeList)
                        {
                            string name = nameBiomeData.Key;
                            if(!nameClassIds.ContainsKey(name))
                            {
                                if(TechTypeExtensions.FromString(name, out TechType techType, true) && techTypeName.TryGetValue(techType, out List<string> techNames))
                                    name = techNames.FirstOrDefault((x) => nameClassIds.ContainsKey(x));
                                else if(TechTypeHandler.TryGetModdedTechType(name, out techType) && techTypeName.TryGetValue(techType, out techNames))
                                    name = techNames.FirstOrDefault((x) => nameClassIds.ContainsKey(x));

                                if(string.IsNullOrEmpty(name) || !nameClassIds.ContainsKey(name))
                                {
                                    Logger.Log(Logger.Level.Warn, "nameClassIds has no " + name);
                                    continue;
                                }
                            }
                            if(!modifiedDistributions.ContainsKey(name))
                            {
                                modifiedDistributions[name] = nameBiomeData.Value;
                                succeded++;
                            }
                            else
                            {
                                List<BiomeData> savedData = modifiedDistributions[name];
                                nameBiomeData.Value.ForEach((x) => savedData.Add(x));
                                savedData.Distinct();
                                succeded++;
                            }
                        }
                        if(succeded > 0)
                            Logger.Log(Logger.Level.Debug, $"Successfully loaded file: {file.Name} with {succeded} TechTypes being altered.");
                        else
                            Logger.Log(Logger.Level.Debug, $"Loaded file: {file.Name} But found {succeded} TechTypes being altered.");
                    }
                    else
                    {
                        Logger.Log(Logger.Level.Debug, $"Ignored file: {file.Name} as it contains the keyword 'disabled' in the name.");
                    }
                }
                catch(Exception e)
                {
                    Logger.Log(Logger.Level.Error, $"Failed to load {file.Name}.", e);
                }
            }
            if(modifiedDistributions.Count > 0)
                RegisterChanges(modifiedDistributions);
        }

        private class TechTypeConverter: JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((TechType)value).AsString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string v = (string)serializer.Deserialize(reader, typeof(string));
                return TechTypeExtensions.FromString(v, out TechType techType, true)
                    ? techType
                    : TechTypeHandler.TryGetModdedTechType(v, out techType)
                        ? (object)techType
                        : throw new Exception($"Failed to parse {v} into a Techtype");
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TechType);
            }
        }

        private static void RegisterChanges(Dictionary<string, List<BiomeData>> modifiedDistributions)
        {
            foreach(KeyValuePair<string, List<BiomeData>> modifiedDist in modifiedDistributions)
            {
                string name = modifiedDist.Key;

                foreach(string classId in nameClassIds[name])
                {
                    if(classIdPrefab.TryGetValue(classId, out string prefabPath))
                    {
                        if(!WorldEntityDatabase.TryGetInfo(classId, out WorldEntityInfo info))
                        {
                            info = new WorldEntityInfo()
                            {
                                cellLevel = LargeWorldEntity.CellLevel.Medium,
                                classId = classId,
                                localScale = UnityEngine.Vector3.one,
                                prefabZUp = false,
                                slotType = EntitySlot.Type.Medium,
                                techType = nameTechType[name]
                            };
                            WorldEntityDatabaseHandler.AddCustomInfo(classId, info);
                            Logger.Log(Logger.Level.Debug, $"AddCustomInfo for {name}: {prefabPath}");
                        }
                        SrcData data = new SrcData() { prefabPath = prefabPath, distribution = modifiedDist.Value };
                        Logger.Log(Logger.Level.Debug, $"Altering Spawn Locations for {name}. Adding {data.distribution.Count} values");
                        LootDistributionHandler.AddLootDistributionData(classId, data);
                    }
                    else
                    {
                        Logger.Log(Logger.Level.Warn, $"Failed to get PrefabPath for {name}. Skipped editing distribution for {name}");
                    }
                }
            }
        }
    }
}
