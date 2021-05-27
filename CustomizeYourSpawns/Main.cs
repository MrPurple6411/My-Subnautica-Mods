namespace CustomizeYourSpawns
{
    //techType = TechType.Seamoth
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
    public static class Main
    {
        private static readonly string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly DirectoryInfo ChangesPath = Directory.CreateDirectory(ModPath + "/ChangesToLoad");
        private static readonly string DefaultDistributions = ModPath + "/DefaultDistributions.json";
        private static readonly string BiomeDictionary = ModPath + "/BiomeList.json";
        private static readonly string ExampleFile = ModPath + "/ExampleFile.json";
        private static readonly string Names = ModPath + "/Names.json";
        private static readonly Dictionary<string, string> classIdName = new();
        private static readonly Dictionary<string, TechType> nameTechType = new();
        private static readonly Dictionary<TechType, List<string>> techTypeName = new();
        private static readonly Dictionary<string, List<string>> nameClassIds = new();
        private static readonly Dictionary<string, string> classIdPrefab = new();

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
            var names = new HashSet<string>();

            foreach(var prefabFile in PrefabDatabase.prefabFiles)
            {
                var techType = CraftData.GetTechForEntNameExpensive(Path.GetFileName(prefabFile.Value).Replace(".prefab", ""));
                var name = techType.AsString();
                nameTechType[name] = techType;
                if (!techTypeName.TryGetValue(techType, out var techNames))
                {
                    techNames = new List<string>();
                    techTypeName[techType] = techNames;
                }

                if (!techNames.Contains(name))
                    techNames.Add(name);

                if (techType == TechType.None)
                {
                    name = prefabFile.Value ?? "";
                    name = name.Substring(name.LastIndexOf("/", StringComparison.Ordinal) + 1)
                        .Replace(".prefab", "");
                }

                if (string.IsNullOrEmpty(name)) continue;
                
                names.Add(name);
                if (!nameClassIds.ContainsKey(name))
                    nameClassIds[name] = new List<string> {prefabFile.Key};
                else
                    nameClassIds[name].Add(prefabFile.Key);

                classIdName[prefabFile.Key] = name;
                classIdPrefab[prefabFile.Key] = prefabFile.Value;

            }

            var modPreFabsList = Traverse.Create<ModPrefab>().Field<List<ModPrefab>>("PreFabsList").Value;

            foreach(var modPrefab in modPreFabsList)
            {
                var name = modPrefab.TechType.AsString();
                var techType = modPrefab.TechType;

                nameTechType[name] = techType;
                if(!techTypeName.TryGetValue(techType, out var techNames))
                {
                    techNames = new List<string>();
                    techTypeName[techType] = techNames;
                }

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

            using var writer = new StreamWriter(Names);
            writer.Write(JsonConvert.SerializeObject(names, Formatting.Indented));
        }

        private static void EnsureDefaultDistributions()
        {
            if(File.Exists(DefaultDistributions))
                return;

            var defaultDistributions = new SortedDictionary<string, Dictionary<BiomeType, BiomeData>>();
            var lootDistData = LootDistributionData.Load("Balance/EntityDistributions");

            foreach(var srcDist in lootDistData.srcDistribution)
            {
                if(srcDist.Key != "None")
                {
                    var classId = srcDist.Key;
                    if(!classIdName.ContainsKey(classId))
                    {
                        Logger.Log(Logger.Level.Warn, "classIdName has no " + classId);
                    }
                    else
                    {
                        var name = classIdName[classId];
                        //string techType1 = "";
                        //if (nameTechType.ContainsKey(name))
                        //    techType1 = nameTechType[name].AsString();

                        //Logger.Log(Logger.Level.Info, "lootDistData " + techType1 + " " + classId + " " + srcDist.Value.prefabPath);
                        //foreach (BiomeData biomeData in srcDist.Value.distribution)
                        //    Logger.Log(Logger.Level.Info, biomeData.biome + " " + biomeData.count + " " + biomeData.probability);

                        if(!defaultDistributions.ContainsKey(name))
                        {
                            var biomeDict = new Dictionary<BiomeType, BiomeData>();
                            foreach(var biomeData in srcDist.Value.distribution)
                                biomeDict[biomeData.biome] = biomeData;

                            defaultDistributions[name] = new Dictionary<BiomeType, BiomeData>(biomeDict);
                        }
                        else
                        {
                            foreach(var srcBiomeData in srcDist.Value.distribution)
                            {
                                if(!defaultDistributions[name].ContainsKey(srcBiomeData.biome))
                                {
                                    defaultDistributions[name][srcBiomeData.biome] = srcBiomeData;
                                }
                                else
                                {
                                    var savedBiomeData = defaultDistributions[name][srcBiomeData.biome];
                                    //Logger.Log(Logger.Level.Info, techType + " same biome " + srcBiomeData.biome);
                                    var newBiomeData = new BiomeData()
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

            var defaultDistributionsL = new SortedDictionary<string, List<BiomeData>>();

            foreach(var defaultDistribution in defaultDistributions)
            {
                var biomeDataList = defaultDistribution.Value.Select(biomeData => biomeData.Value).ToList();

                defaultDistributionsL[defaultDistribution.Key] = new List<BiomeData>(biomeDataList);
            }

            using var writer = new StreamWriter(DefaultDistributions);
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

        private static void EnsureBiomeDictionary()
        {
            if (File.Exists(BiomeDictionary)) return;
            var biomeDictionary = new SortedDictionary<int, string>();
            foreach(BiomeType biome in Enum.GetValues(typeof(BiomeType)))
                biomeDictionary[(int)biome] = biome.AsString();

            using var writer = new StreamWriter(BiomeDictionary);
            writer.Write(JsonConvert.SerializeObject(biomeDictionary, Formatting.Indented));
        }

        private static void EnsureExample()
        {
            if (File.Exists(ExampleFile)) return;
            var example = new Dictionary<TechType, List<BiomeData>>() {
                {
                    TechType.GenericJeweledDisk,
                    new List<BiomeData>()
                    {
                        new()
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
                        new()
                        {
                            biome = BiomeType.SafeShallows_Grass,
                            count = 4,
                            probability = 1
                        },
                        new()
                        {
                            biome = BiomeType.SafeShallows_Plants,
                            count = 6,
                            probability = 1
                        }
                    }

                }
            };
            using var writer = new StreamWriter(ExampleFile);
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

        private static void LoadChangeFiles()
        {
            var modifiedDistributions = new Dictionary<string, List<BiomeData>>();
            foreach(var file in ChangesPath.GetFiles().Where((x) => x.Extension.ToLower() == ".json"))
            {
                try
                {
                    if (file.Name.ToLower().Contains("disabled"))
                    {
                        Logger.Log(Logger.Level.Debug,
                            $"Ignored file: {file.Name} as it contains the keyword 'disabled' in the name.");
                        continue;
                    }
                    
                    SortedDictionary<string, List<BiomeData>> nameBiomeList;
                    using (var reader = new StreamReader(file.FullName))
                    {
                        nameBiomeList = JsonConvert.DeserializeObject<SortedDictionary<string, List<BiomeData>>>(
                            reader.ReadToEnd(), new JsonConverter[]
                            {
                                new StringEnumConverter()
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

                    var succeeded = 0;

                    foreach (var nameBiomeData in nameBiomeList)
                    {
                        var name = nameBiomeData.Key;
                        if (!nameClassIds.ContainsKey(name))
                        {
                            if (TechTypeExtensions.FromString(name, out var techType, true) &&
                                techTypeName.TryGetValue(techType, out var techNames))
                                name = techNames.FirstOrDefault((x) => nameClassIds.ContainsKey(x));
                            else if (TechTypeHandler.TryGetModdedTechType(name, out techType) &&
                                     techTypeName.TryGetValue(techType, out techNames))
                                name = techNames.FirstOrDefault((x) => nameClassIds.ContainsKey(x));

                            if (string.IsNullOrEmpty(name) || !nameClassIds.ContainsKey(name))
                            {
                                Logger.Log(Logger.Level.Warn, "nameClassIds has no " + name);
                                continue;
                            }
                        }

                        if (!modifiedDistributions.ContainsKey(name))
                        {
                            modifiedDistributions[name] = nameBiomeData.Value;
                            succeeded++;
                        }
                        else
                        {
                            var savedData = modifiedDistributions[name];
                            nameBiomeData.Value.ForEach((x) => savedData.Add(x));
                            savedData = savedData.Distinct().ToList();
                            modifiedDistributions[name] = savedData;
                            succeeded++;
                        }
                    }

                    Logger.Log(Logger.Level.Debug,
                        succeeded > 0
                            ? $"Successfully loaded file: {file.Name} with {succeeded} TechTypes being altered."
                            : $"Loaded file: {file.Name} But found {succeeded} TechTypes being altered.");
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
                var v = (string)serializer.Deserialize(reader, typeof(string));
                return TechTypeExtensions.FromString(v, out var techType, true)
                    ? techType
                    : TechTypeHandler.TryGetModdedTechType(v, out techType)
                        ? (object)techType
                        : throw new Exception($"Failed to parse {v} into a TechType");
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TechType);
            }
        }

        private static void RegisterChanges(Dictionary<string, List<BiomeData>> modifiedDistributions)
        {
            foreach(var modifiedDist in modifiedDistributions)
            {
                var name = modifiedDist.Key;

                foreach(var classId in nameClassIds[name])
                {
                    if(classIdPrefab.TryGetValue(classId, out var prefabPath))
                    {
                        if(!WorldEntityDatabase.TryGetInfo(classId, out var info))
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
                        var data = new SrcData() { prefabPath = prefabPath, distribution = modifiedDist.Value };
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
