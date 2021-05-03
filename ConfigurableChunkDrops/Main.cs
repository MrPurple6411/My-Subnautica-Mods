namespace ConfigurableChunkDrops
{
    using ConfigurableChunkDrops.Configuration;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;

    [QModCore]
    public static class Main
    {
        internal static Config config = new Config();
        internal static string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static Config defaultValues = new Config("DefaultValues");

        [QModPatch]
        public static void Load()
        {
            config.Load();

            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        internal static IEnumerator GenerateDefaults()
        {
            Logger.Log(Logger.Level.Info, "Generating Default File", showOnScreen: true);

            foreach(TechType techType in Enum.GetValues(typeof(TechType)))
            {

                CoroutineTask<GameObject> prefabForTechTypeAsync = CraftData.GetPrefabForTechTypeAsync(techType, false);
                yield return prefabForTechTypeAsync;

                GameObject prefab = prefabForTechTypeAsync?.GetResult();

                // if techtype has no prefab no need to check further.
                if(prefab is null)
                    continue;

                BreakableResource breakableResource = prefab.GetComponentInChildren<BreakableResource>();

                // if techtype has no BreakableResource no need to check further.
                if(breakableResource is null)
                    continue;

                Dictionary<TechType, float> prefabs = defaultValues.Breakables[techType] = new Dictionary<TechType, float>();
                foreach(BreakableResource.RandomPrefab randomPrefab in breakableResource.prefabList)
                {
#if SUBNAUTICA_STABLE
                    TechType tech = CraftData.GetTechType(randomPrefab.prefab);
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
            yield break;
        }
    }
}