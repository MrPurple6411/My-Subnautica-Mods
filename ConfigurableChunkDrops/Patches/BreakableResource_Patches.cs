using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace ConfigurableChunkDrops.Patches
{
    [HarmonyPatch(typeof(BreakableResource), "BreakIntoResources")]
    public static class BreakableResource_ChooseRandomResource
    {

        private static Dictionary<TechType, List<BreakableResource.RandomPrefab>> prefabs = new Dictionary<TechType, List<BreakableResource.RandomPrefab>>();

        [HarmonyPrefix]
        public static void Prefix(BreakableResource __instance)
        {
            Main.config.Load();

            TechType Breakable = CraftData.GetTechType(__instance.gameObject);

            if (!Main.config.Breakables.ContainsKey(Breakable))
                return;

            Dictionary<TechType, float> customDrops = Main.config.Breakables[Breakable];

            if (prefabs.ContainsKey(Breakable) && prefabs.Keys.ToList() == customDrops.Keys.ToList())
            {
                __instance.prefabList = prefabs[Breakable];
                return;
            }
            
            __instance.prefabList = new List<BreakableResource.RandomPrefab>();

            foreach(KeyValuePair<TechType, float> pair in customDrops)
            {
                PlayerEntropy entropy = Player.main.gameObject.GetComponent<PlayerEntropy>();
                PlayerEntropy.TechEntropy techEntropy = entropy.randomizers.Find((x) => x.techType == pair.Key);

                if (techEntropy is null)
                    entropy.randomizers.Add(new PlayerEntropy.TechEntropy() { entropy = new FairRandomizer(), techType = pair.Key });

                CoroutineHost.StartCoroutine(GetPrefabForList(Breakable, pair.Key, pair.Value));
            }

        }

        private static IEnumerator GetPrefabForList(TechType breakable, TechType techType, float chance)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType, false);

            if (prefabs.ContainsKey(breakable))
            {
                BreakableResource.RandomPrefab existingPrefab = prefabs[breakable].Find((x) => CraftData.GetTechType(x.prefab) == techType);
                if (existingPrefab != null)
                {
                    existingPrefab.chance = chance;
                    yield break;
                }
                else
                {
                    yield return task;
                    GameObject gameObject = task.GetResult();
                    if (gameObject != null)
                        prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefab = gameObject, chance = chance });
                    yield break;
                }
            }
            else
            {
                prefabs[breakable] = new List<BreakableResource.RandomPrefab>();
                yield return task;
                GameObject gameObject = task.GetResult();
                if (gameObject != null)
                    prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefab = gameObject, chance = chance });
                yield break;
            }
        }
    }
}
