namespace ConfigurableChunkDrops.Patches
{
#if !SUBNAUTICA_STABLE
    using UnityEngine.AddressableAssets;
#endif
    using HarmonyLib;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UWE;

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Player_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            foreach(KeyValuePair<TechType, Dictionary<TechType, float>> breakable in Main.config.Breakables)
            {
                foreach(KeyValuePair<TechType, float> pair in breakable.Value)
                {
                    PlayerEntropy entropy = __instance.gameObject.GetComponent<PlayerEntropy>();
                    PlayerEntropy.TechEntropy techEntropy = entropy.randomizers.Find((x) => x.techType == pair.Key);

                    if(techEntropy is null)
                        entropy.randomizers.Add(new PlayerEntropy.TechEntropy() { entropy = new FairRandomizer(), techType = pair.Key });

                    CoroutineHost.StartCoroutine(GetPrefabForList(breakable.Key, pair.Key, pair.Value));
                }
            }
        }

        private static IEnumerator GetPrefabForList(TechType breakable, TechType techType, float chance)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType, false);

#if SUBNAUTICA_STABLE
            if(BreakableResource_ChooseRandomResource.prefabs.ContainsKey(breakable))
            {

                BreakableResource.RandomPrefab existingPrefab = BreakableResource_ChooseRandomResource.prefabs[breakable].Find((x) => CraftData.GetTechType(x.prefab) == techType);
                if(existingPrefab != null)
                {
                    existingPrefab.chance = chance;
                    yield break;
                }
                else
                {
                    yield return task;
                    GameObject gameObject = task.GetResult();
                    if(gameObject != null)
                        BreakableResource_ChooseRandomResource.prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefab = gameObject, chance = chance });
                    yield break;
                }
            }
            else
            {
                BreakableResource_ChooseRandomResource.prefabs[breakable] = new List<BreakableResource.RandomPrefab>();
                yield return task;
                GameObject gameObject = task.GetResult();
                if(gameObject != null)
                    BreakableResource_ChooseRandomResource.prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefab = gameObject, chance = chance });
                yield break;
            }
#else

            if (!BreakableResource_ChooseRandomResource.prefabs.ContainsKey(breakable))
                BreakableResource_ChooseRandomResource.prefabs[breakable] = new List<BreakableResource.RandomPrefab>();

            BreakableResource.RandomPrefab existingPrefab = BreakableResource_ChooseRandomResource.prefabs[breakable].Find((x) => x.prefabTechType == techType);
            if (existingPrefab != null)
            {
                existingPrefab.chance = chance;
                yield break;
            }

            yield return task;
            GameObject gameObject = task.GetResult();
            if (gameObject != null && PrefabDatabase.TryGetPrefabFilename(gameObject.GetComponent<PrefabIdentifier>().ClassId, out string filename))
                BreakableResource_ChooseRandomResource.prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefabTechType = techType, prefabReference = new AssetReferenceGameObject(filename), chance = chance });
            yield break;
#endif
        }
    }
}
