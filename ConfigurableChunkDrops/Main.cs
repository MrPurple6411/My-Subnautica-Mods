namespace ConfigurableChunkDrops
{
    using Configuration;
    using HarmonyLib;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using BepInEx;
    using BepInEx.Logging;
    using UWE;
    using Newtonsoft.Json;
    using SMLHelper.V2.Handlers;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [HarmonyPatch]
    public class Main: BaseUnityPlugin
    {
        internal static readonly SMLConfig smlConfig = new();
        private static readonly SMLConfig defaultValues = new(defaultsFilename);

        #region[Declarations]

        public const string
            MODNAME = "ConfigurableChunkDrops",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0",
            defaultsFilename = "DefaultValues";

        internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static ManualLogSource logSource;

        #endregion

        private void Awake()
        {
            logSource = Logger;

            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);


        }

        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPrefix]
        public static void Prefix()
        {
            if(!File.Exists(Path.Combine(Paths.ConfigPath, MODNAME, defaultsFilename+".json")))
            {
                CoroutineHost.StartCoroutine(GenerateDefaults());
            }
        }

        internal static IEnumerator GenerateDefaults()
        {
            logSource.Log(LogLevel.Info, "Generating Default File");

            foreach(TechType techType in Enum.GetValues(typeof(TechType)))
            {

                var prefabForTechTypeAsync = CraftData.GetPrefabForTechTypeAsync(techType, false);
                yield return prefabForTechTypeAsync;

                var prefab = prefabForTechTypeAsync?.GetResult();

                var breakableResource = prefab != null ? prefab.GetComponentInChildren<BreakableResource>() : null;

                // if TechType has no BreakableResource no need to check further.
                if(breakableResource is null)
                    continue;

                var prefabs = defaultValues.Breakables[techType.AsString()] = new Dictionary<string, float>();
                foreach(var randomPrefab in breakableResource.prefabList)
                {
                    prefabs[randomPrefab.prefabTechType.AsString()] = randomPrefab.chance;
                }

                prefabs.Add(breakableResource.defaultPrefabTechType.AsString(), 1f);
                logSource.Log(LogLevel.Info, $"Added {Language.main.GetOrFallback(techType.AsString(), techType.AsString())} to Defaults File");
                defaultValues.Save();
            }
            logSource.Log(LogLevel.Info, "Default File Complete");
        }
    }
}