namespace ConfigurableChunkDrops.Patches;

using UnityEngine.AddressableAssets;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UWE;
using ConfigurableChunkDrops.Configuration;
using BepInEx;
using Newtonsoft.Json;
using System.IO;
using Nautilus.Handlers;
using System;
using Nautilus.Extensions;
using UnityEngine;

[HarmonyPatch(typeof(Player), nameof(Player.Awake))]
public static class Player_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Player __instance)
    {
        Main.smlConfig.Load();

		var entropy = __instance.gameObject.GetComponent<PlayerEntropy>();

		foreach (var breakable in Main.smlConfig.Breakables)
        {
            if(TechTypeExtensions.FromString(breakable.Key, out TechType breakableType, true))
            {
                foreach(var pair in breakable.Value)
                {
                    if(TechTypeExtensions.FromString(pair.Key, out TechType dropType, true))
                    {
						if (dropType == TechType.Cyclops)
						{
							Main.logSource.LogError("Cyclops is not a valid drop type. Skipping.");
							continue;
						}
                        var techEntropy = entropy.randomizers.Find((x) => x.techType == dropType);

                        if(techEntropy == default)
                            entropy.randomizers.Add(new PlayerEntropy.TechEntropy() { entropy = __instance.gameObject.AddComponent<FairRandomizer>(), techType = dropType });

                        CoroutineHost.StartCoroutine(GetPrefabForList(breakableType, dropType, pair.Value));
                    }
                }
            }
        }
    }

    private static IEnumerator GetPrefabForList(TechType breakable, TechType techType, float newChance)
    {
        var task = CraftData.GetPrefabForTechTypeAsync(techType, false);

        if (!BreakableResource_ChooseRandomResource.prefabs.ContainsKey(breakable))
            BreakableResource_ChooseRandomResource.prefabs[breakable] = new List<BreakableResource.RandomPrefab>();

        var existingPrefab = BreakableResource_ChooseRandomResource.prefabs[breakable].Find((x) => x.prefabTechType == techType);
        if (existingPrefab != null)
        {
            existingPrefab.chance = newChance;
            yield break;
        }

        yield return task;
        var gameObject = task.GetResult();
        if (gameObject != null && PrefabDatabase.TryGetPrefabFilename(gameObject.GetComponent<PrefabIdentifier>().ClassId, out var filename))
            BreakableResource_ChooseRandomResource.prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefabTechType = techType, prefabReference = new AssetReferenceGameObject(filename).ForceValid(), chance = newChance });
    }
}
