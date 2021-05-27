namespace ConfigurableChunkDrops.Patches
{
#if !SUBNAUTICA_STABLE
    using UnityEngine.AddressableAssets;
#endif
    using HarmonyLib;
    using System.Collections;
    using System.Collections.Generic;
    using UWE;

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Player_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            foreach(var breakable in Main.config.Breakables)
            {
                foreach(var pair in breakable.Value)
                {
                    var entropy = __instance.gameObject.GetComponent<PlayerEntropy>();
                    var techEntropy = entropy.randomizers.Find((x) => x.techType == pair.Key);

                    if(techEntropy == default)
                        entropy.randomizers.Add(new PlayerEntropy.TechEntropy() { entropy = __instance.gameObject.AddComponent<FairRandomizer>(), techType = pair.Key });

                    CoroutineHost.StartCoroutine(GetPrefabForList(breakable.Key, pair.Key, pair.Value));
                }
            }
        }

        private static IEnumerator GetPrefabForList(TechType breakable, TechType techType, float chance)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(techType, false);
#if SUBNAUTICA_STABLE
            if(BreakableResource_ChooseRandomResource.prefabs.ContainsKey(breakable))
            {
                var existingPrefab = BreakableResource_ChooseRandomResource.prefabs[breakable].Find((x) => CraftData.GetTechType(x.prefab) == techType);
                if(existingPrefab != null)
                {
                    existingPrefab.chance = chance;
                }
                else
                {
                    yield return task;
                    var gameObject = task.GetResult();
                    if(gameObject != null)
                        BreakableResource_ChooseRandomResource.prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefab = gameObject, chance = chance });
                }
            }
            else
            {
                BreakableResource_ChooseRandomResource.prefabs[breakable] = new List<BreakableResource.RandomPrefab>();
                yield return task;
                var gameObject = task.GetResult();
                if(gameObject != null)
                    BreakableResource_ChooseRandomResource.prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefab = gameObject, chance = chance });
            }
#else

            if (!BreakableResource_ChooseRandomResource.prefabs.ContainsKey(breakable))
                BreakableResource_ChooseRandomResource.prefabs[breakable] = new List<BreakableResource.RandomPrefab>();

            var existingPrefab = BreakableResource_ChooseRandomResource.prefabs[breakable].Find((x) => x.prefabTechType == techType);
            if (existingPrefab != null)
            {
                existingPrefab.chance = chance;
                yield break;
            }

            yield return task;
            var gameObject = task.GetResult();
            if (gameObject != null && PrefabDatabase.TryGetPrefabFilename(gameObject.GetComponent<PrefabIdentifier>().ClassId, out var filename))
                BreakableResource_ChooseRandomResource.prefabs[breakable].Add(new BreakableResource.RandomPrefab() { prefabTechType = techType, prefabReference = new AssetReferenceGameObject(filename), chance = chance });
#endif
        }
    }
}
