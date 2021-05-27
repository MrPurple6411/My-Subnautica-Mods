namespace ConfigurableChunkDrops
{
    using Configuration;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Logger = QModManager.Utility.Logger;

    [QModCore]
    public static class Main
    {
        internal static readonly Config config = new();
        internal static readonly string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly Config defaultValues = new("DefaultValues");

        [QModPatch]
        public static void Load()
        {
            config.Load();

            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        internal static IEnumerator GenerateDefaults()
        {
            Logger.Log(Logger.Level.Info, "Generating Default File", showOnScreen: true);

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
#if SUBNAUTICA_STABLE
                    var tech = CraftData.GetTechType(randomPrefab.prefab);
                    prefabs[tech] = randomPrefab.chance;
#else

                    prefabs[randomPrefab.prefabTechType] = randomPrefab.chance;
#endif
                }


#if SUBNAUTICA_STABLE
                prefabs.Add(CraftData.GetTechType(breakableResource.defaultPrefab), 1f);
#else
                prefabs.Add(breakableResource.defaultPrefabTechType, 1f);
#endif
                Logger.Log(Logger.Level.Info, $"Added {Language.main.GetOrFallback(techType.AsString(), techType.AsString())} to Defaults File", showOnScreen: true);
                defaultValues.Save();
            }
            Logger.Log(Logger.Level.Info, "Default File Complete", showOnScreen: true);
        }
    }
}