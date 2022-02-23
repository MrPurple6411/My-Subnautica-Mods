using BepInEx;

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
    using SMCLib.Assets;
    using SMCLib.Handlers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using UWE;
    using static LootDistributionData;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        private static readonly string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly DirectoryInfo ChangesPath = Directory.CreateDirectory(ModPath + "/ChangesToLoad");
        private static readonly string DefaultDistributions = ModPath + "/DefaultDistributions.json";
        private static readonly string BiomeDictionary = ModPath + "/BiomeList.json";
        private static readonly string ExampleFile = ModPath + "/ExampleFile.json";
        private static readonly string Names = ModPath + "/Names.json";
        private static readonly Dictionary<string, string> ClassIdName = new();
        private static readonly Dictionary<string, TechType> NameTechType = new();
        private static readonly Dictionary<TechType, List<string>> TechTypeName = new();
        private static readonly Dictionary<string, List<string>> NameClassIds = new();
        private static readonly Dictionary<string, string> ClassIdPrefab = new();

        public void Start()
        {
            Setup();
            EnsureDefaultDistributions();
            EnsureBiomeDictionary();
            EnsureExample();
            LoadChangeFiles();
        }

        private void Setup()
        {
            PrefabDatabase.LoadPrefabDatabase(SNUtils.prefabDatabaseFilename);
            var names = new List<string>();

            foreach(var prefabFile in PrefabDatabase.prefabFiles)
            {
                var techType = CraftData.GetTechForEntNameExpensive(Path.GetFileName(prefabFile.Value).Replace(".prefab", ""));
                var techTypeString = techType.AsString();
                NameTechType[techTypeString] = techType;
                if (!TechTypeName.TryGetValue(techType, out var techNames))
                {
                    techNames = new List<string>();
                    TechTypeName[techType] = techNames;
                }

                if (!techNames.Contains(techTypeString))
                    techNames.Add(techTypeString);

                if (techType == TechType.None)
                {
                    techTypeString = prefabFile.Value ?? "";
                    techTypeString = techTypeString.Substring(techTypeString.LastIndexOf("/", StringComparison.Ordinal) + 1)
                        .Replace(".prefab", "");
                }

                if (string.IsNullOrEmpty(techTypeString)) continue;
                
                names.Add(techTypeString);
                if (!NameClassIds.ContainsKey(techTypeString))
                    NameClassIds[techTypeString] = new List<string> {prefabFile.Key};
                else
                    NameClassIds[techTypeString].Add(prefabFile.Key);

                ClassIdName[prefabFile.Key] = techTypeString;
                ClassIdPrefab[prefabFile.Key] = prefabFile.Value;

            }

            var modPreFabsList = Traverse.Create<ModPrefab>().Field<List<ModPrefab>>("PreFabsList").Value;

            foreach(var modPrefab in modPreFabsList)
            {
                var techTypeString = modPrefab.TechType.AsString();
                var techType = modPrefab.TechType;

                NameTechType[techTypeString] = techType;
                if(!TechTypeName.TryGetValue(techType, out var techNames))
                {
                    techNames = new List<string>();
                    TechTypeName[techType] = techNames;
                }

                if(!techNames.Contains(techTypeString))
                    techNames.Add(techTypeString);

                names.Add(techTypeString);
                if(!NameClassIds.ContainsKey(techTypeString))
                    NameClassIds[techTypeString] = new List<string> { modPrefab.ClassID };
                else
                    NameClassIds[techTypeString].Add(modPrefab.ClassID);

                ClassIdName[modPrefab.ClassID] = techTypeString;
                ClassIdPrefab[modPrefab.ClassID] = modPrefab.PrefabFileName;

            }

            using var writer = new StreamWriter(Names);
            writer.Write(JsonConvert.SerializeObject(names, Formatting.Indented));
        }

        private  void EnsureDefaultDistributions()
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
                    if(!ClassIdName.ContainsKey(classId))
                    {
                        Logger.LogWarning("classIdName has no " + classId);
                    }
                    else
                    {
                        var classIdName = ClassIdName[classId];
                        //string techType1 = "";
                        //if (nameTechType.ContainsKey(name))
                        //    techType1 = nameTechType[name].AsString();

                        //Main.logSource.LogInfo("lootDistData " + techType1 + " " + classId + " " + srcDist.Value.prefabPath);
                        //foreach (BiomeData biomeData in srcDist.Value.distribution)
                        //    Main.logSource.LogInfo(biomeData.biome + " " + biomeData.count + " " + biomeData.probability);

                        if(!defaultDistributions.ContainsKey(classIdName))
                        {
                            var biomeDict = new Dictionary<BiomeType, BiomeData>();
                            foreach(var biomeData in srcDist.Value.distribution)
                                biomeDict[biomeData.biome] = biomeData;

                            defaultDistributions[classIdName] = new Dictionary<BiomeType, BiomeData>(biomeDict);
                        }
                        else
                        {
                            foreach(var srcBiomeData in srcDist.Value.distribution)
                            {
                                if(!defaultDistributions[classIdName].ContainsKey(srcBiomeData.biome))
                                {
                                    defaultDistributions[classIdName][srcBiomeData.biome] = srcBiomeData;
                                }
                                else
                                {
                                    var savedBiomeData = defaultDistributions[classIdName][srcBiomeData.biome];
                                    //Main.logSource.LogInfo(techType + " same biome " + srcBiomeData.biome);
                                    var newBiomeData = new BiomeData()
                                    {
                                        biome = srcBiomeData.biome,
                                        count = srcBiomeData.count + savedBiomeData.count,
                                        probability = (srcBiomeData.probability + savedBiomeData.probability) * 0.5f
                                    };
                                    defaultDistributions[classIdName][srcBiomeData.biome] = newBiomeData;
                                }
                            }
                        }
                    }
                }
            }
            Logger.LogInfo("defaultDistributions count  " + defaultDistributions.Count);

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

        private void EnsureBiomeDictionary()
        {
            if (File.Exists(BiomeDictionary)) return;
            var biomeDictionary = new SortedDictionary<int, string>();
            foreach(BiomeType biome in Enum.GetValues(typeof(BiomeType)))
                biomeDictionary[(int)biome] = biome.AsString();

            using var writer = new StreamWriter(BiomeDictionary);
            writer.Write(JsonConvert.SerializeObject(biomeDictionary, Formatting.Indented));
        }

        private void EnsureExample()
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

        private void LoadChangeFiles()
        {
            var modifiedDistributions = new Dictionary<string, List<BiomeData>>();
            foreach(var file in ChangesPath.GetFiles().Where((x) => x.Extension.ToLower() == ".json"))
            {
                try
                {
                    if (file.Name.ToLower().Contains("disabled"))
                    {
                        Logger.LogDebug($"Ignored file: {file.Name} as it contains the keyword 'disabled' in the name.");
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
                        var key = nameBiomeData.Key;
                        if (!NameClassIds.ContainsKey(key))
                        {
                            if (TechTypeExtensions.FromString(key, out var techType, true) &&
                                TechTypeName.TryGetValue(techType, out var techNames))
                                key = techNames.FirstOrDefault((x) => NameClassIds.ContainsKey(x));
                            else if (TechTypeHandler.TryGetModdedTechType(key, out techType) &&
                                     TechTypeName.TryGetValue(techType, out techNames))
                                key = techNames.FirstOrDefault((x) => NameClassIds.ContainsKey(x));

                            if (string.IsNullOrEmpty(key) || !NameClassIds.ContainsKey(key))
                            {
                                Logger.LogWarning("nameClassIds has no " + key);
                                continue;
                            }
                        }

                        if (!modifiedDistributions.ContainsKey(key))
                        {
                            modifiedDistributions[key] = nameBiomeData.Value;
                            succeeded++;
                        }
                        else
                        {
                            var savedData = modifiedDistributions[key];
                            nameBiomeData.Value.ForEach((x) => savedData.Add(x));
                            savedData = savedData.Distinct().ToList();
                            modifiedDistributions[key] = savedData;
                            succeeded++;
                        }
                    }

                    Logger.LogDebug(
                        succeeded > 0
                            ? $"Successfully loaded file: {file.Name} with {succeeded} TechTypes being altered."
                            : $"Loaded file: {file.Name} But found {succeeded} TechTypes being altered.");
                }
                catch(Exception e)
                {
                    Logger.LogError($"Failed to load {file.Name}.\n{e}");
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

        private void RegisterChanges(Dictionary<string, List<BiomeData>> modifiedDistributions)
        {
            foreach(var modifiedDist in modifiedDistributions)
            {
                var key = modifiedDist.Key;

                foreach(var classId in NameClassIds[key])
                {
                    if(ClassIdPrefab.TryGetValue(classId, out var prefabPath))
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
                                techType = NameTechType[key]
                            };
                            WorldEntityDatabaseHandler.AddCustomInfo(classId, info);
                            Logger.LogDebug( $"AddCustomInfo for {key}: {prefabPath}");
                        }
                        var data = new SrcData() { prefabPath = prefabPath, distribution = modifiedDist.Value };
                        Logger.LogDebug($"Altering Spawn Locations for {key}. Adding {data.distribution.Count} values");
                        LootDistributionHandler.AddLootDistributionData(classId, data);
                    }
                    else
                    {
                        Logger.LogWarning($"Failed to get PrefabPath for {key}. Skipped editing distribution for {key}");
                    }
                }
            }
        }
    }
}
