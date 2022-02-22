using BepInEx;
using BepInEx.Logging;

namespace ConfigurableChunkDrops
{
    using Configuration;
    using HarmonyLib;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public  class Main:BaseUnityPlugin
    {
        internal static readonly Config config = new();
        internal static readonly string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly Config defaultValues = new("DefaultValues");
        internal static ManualLogSource logSource;
        
        public void Awake()
        {
            logSource = Logger;
            config.Load();

            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        internal static IEnumerator GenerateDefaults()
        {
            logSource.LogInfo("Generating Default File");
            ErrorMessage.AddMessage("Generating Default File");

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
                logSource.LogInfo($"Added {Language.main.GetOrFallback(techType.AsString(), techType.AsString())} to Defaults File");
                ErrorMessage.AddMessage($"Added {Language.main.GetOrFallback(techType.AsString(), techType.AsString())} to Defaults File");
                defaultValues.Save();
            }
            logSource.LogInfo("Default File Complete");
            ErrorMessage.AddMessage("Default File Complete");
        }
    }
}