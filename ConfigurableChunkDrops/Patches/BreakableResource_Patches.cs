using HarmonyLib;
using System.Collections.Generic;

namespace ConfigurableChunkDrops.Patches
{
    [HarmonyPatch(typeof(BreakableResource), "BreakIntoResources")]
    public static class BreakableResource_ChooseRandomResource
    {
        [HarmonyPrefix]
        public static void Prefix(BreakableResource __instance)
        {
            Main.config.Load();

            TechType Breakable = CraftData.GetTechType(__instance.gameObject);

            if (!Main.config.Breakables.ContainsKey(Breakable))
                return;

            Dictionary<TechType, float> customDrops = Main.config.Breakables[Breakable];

            __instance.prefabList = new List<BreakableResource.RandomPrefab>();

            foreach(KeyValuePair<TechType, float> pair in customDrops)
            {
                PlayerEntropy entropy = Player.main.gameObject.GetComponent<PlayerEntropy>();
                PlayerEntropy.TechEntropy techEntropy = entropy.randomizers.Find((x) => x.techType == pair.Key);

                if (techEntropy is null)
                    entropy.randomizers.Add(new PlayerEntropy.TechEntropy() { entropy = new FairRandomizer(), techType = pair.Key });

                BreakableResource.RandomPrefab randomPrefab = new BreakableResource.RandomPrefab() { prefab = CraftData.InstantiateFromPrefab(pair.Key), chance = pair.Value };
                BreakableResource.RandomPrefab existingRandomPrefab = __instance.prefabList.Find((x) => x.prefab.name == randomPrefab.prefab.name);

                if (existingRandomPrefab is null)
                    __instance.prefabList.Add(randomPrefab);
                else
                    existingRandomPrefab.chance = pair.Value;
            }

        }
    }
}
