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

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static readonly Config config = new();
        internal static readonly string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly Config defaultValues = new("DefaultValues");

        #region[Declarations]

        public const string
            MODNAME = "ConfigurableChunkDrops",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static ManualLogSource logSource;

        #endregion

        private void Awake()
        {
            logSource = Logger;
            config.Load();

            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPrefix]
        public static void Prefix()
        {
            if(!File.Exists(Path.Combine(Main.modPath, "DefaultValues.json")))
                CoroutineHost.StartCoroutine(GenerateDefaults());
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

                var prefabs = defaultValues.Breakables[techType] = new Dictionary<TechType, float>();
                foreach(var randomPrefab in breakableResource.prefabList)
                {
                    prefabs[randomPrefab.prefabTechType] = randomPrefab.chance;
                }

                prefabs.Add(breakableResource.defaultPrefabTechType, 1f);
                logSource.Log(LogLevel.Info, $"Added {Language.main.GetOrFallback(techType.AsString(), techType.AsString())} to Defaults File");
                defaultValues.Save();
            }
            logSource.Log(LogLevel.Info, "Default File Complete");
        }
    }
}