using HarmonyLib;
using Oculus.Newtonsoft.Json;
using Oculus.Newtonsoft.Json.Converters;
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
        internal static Assembly assembly = Assembly.GetExecutingAssembly();
        internal static string modPath = Path.GetDirectoryName(assembly.Location);
        internal static DirectoryInfo workingPath = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ChangesToLoad"));
        internal static string DefaultDistributions = modPath + "/DefaultDistributions.json";
        internal static string BiomeDictionary = modPath + "/BiomeList.json";
        internal static string ExampleFile = modPath + "/ExampleFile.json";
        internal static Dictionary<TechType, List<BiomeData>> modifiedDistributions = new Dictionary<TechType, List<BiomeData>>();

        [QModPostPatch]
        public static void Load()
        {
            if (!File.Exists(DefaultDistributions) || !File.Exists(BiomeDictionary) || !File.Exists(ExampleFile))
            {
                CreateResources();
            }

            LoadDistributions();
        }

        private static void LoadDistributions()
        {
            foreach(FileInfo file in workingPath.GetFiles().Where((x)=>x.Extension.ToLower()== ".json"))
            {
                try
                {
                    if (!file.Name.ToLower().Contains("disabled"))
                    {
                        SortedDictionary<TechType, List<BiomeData>> pairs;
                        using (StreamReader reader = new StreamReader(file.FullName))
                        {
                            pairs = JsonConvert.DeserializeObject<SortedDictionary<TechType, List<BiomeData>>>(reader.ReadToEnd(), new StringEnumConverter() { CamelCaseText = true, AllowIntegerValues = true }) ?? new SortedDictionary<TechType, List<BiomeData>>();
                        }
                        foreach (KeyValuePair<TechType, List<BiomeData>> pair in pairs)
                        {
                            if (modifiedDistributions.ContainsKey(pair.Key))
                            {
                                List<BiomeData> datas = modifiedDistributions[pair.Key];
                                pair.Value.ForEach((x) => datas.Add(x));
                                datas.Distinct();
                            }
                            else
                            {
                                modifiedDistributions[pair.Key] = pair.Value;
                            }
                        }
                        Logger.Log(Logger.Level.Debug, $"Successfully loaded file: {file.Name} with {pairs.Count} TechTypes being altered.");
                    }
                    else
                    {
                        Logger.Log(Logger.Level.Debug, $"Ignored file: {file.Name} as it contains the keyword 'disabled' in the name.");
                    }
                }
                catch (Exception e)
                {
                    if(e is JsonSerializationException)
                    {
                        Logger.Log(Logger.Level.Error, $"JsonSerialization failed for file {file.Name}.  I suggest running it through a json validator to find the flaw.");
                    }
                    Logger.Log(Logger.Level.Error, $"Failed to load {file.Name}.", e);
                }
            }

            foreach (KeyValuePair<TechType, List<BiomeData>> pair in modifiedDistributions)
            {
                string classId = CraftData.GetClassIdForTechType(pair.Key);
                if (PrefabDatabase.TryGetPrefabFilename(classId, out string prefabPath))
                {
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

        internal static void CreateResources()
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
                    writer.Write(JsonConvert.SerializeObject(defaultDistributions, Formatting.Indented, new StringEnumConverter() { CamelCaseText = true, AllowIntegerValues = true }));
                }
            }

            if (!File.Exists(BiomeDictionary))
            {

                SortedDictionary< int, string>  biomeDictionary = new SortedDictionary<int,string>();
                foreach(BiomeType biome in Enum.GetValues(typeof(BiomeType)))
                {
                    biomeDictionary[(int)biome] = biome.AsString();
                }

                using (StreamWriter writer = new StreamWriter(BiomeDictionary))
                {
                    writer.Write(JsonConvert.SerializeObject(biomeDictionary, Formatting.Indented));
                }
            }

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
                    writer.Write(JsonConvert.SerializeObject(example, Formatting.Indented, new StringEnumConverter() { CamelCaseText = true, AllowIntegerValues = true }));
                }
            }
        }
    }
}