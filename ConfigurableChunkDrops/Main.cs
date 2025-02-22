namespace ConfigurableChunkDrops;

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

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
[HarmonyPatch]
public class Main: BaseUnityPlugin
{
    public const string
        defaultsFilename = "DefaultValues";
    internal static ManualLogSource logSource;
    internal static readonly SMLConfig smlConfig = new();
    private static readonly SMLConfig defaultValues = new(defaultsFilename);

    private void Awake()
    {
        logSource = Logger;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPrefix]
    public static void Prefix()
    {
        if(!File.Exists(Path.Combine(Paths.ConfigPath, MyPluginInfo.PLUGIN_NAME, defaultsFilename+".json")))
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