namespace ConfigurableChunkDrops.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;

    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    public static class BreakableResource_ChooseRandomResource
    {
        public static Dictionary<TechType, List<BreakableResource.RandomPrefab>> prefabs = new();

        [HarmonyPrefix]
        public static void Prefix(BreakableResource __instance)
        {
            var Breakable = CraftData.GetTechType(__instance.gameObject);

            if(prefabs.TryGetValue(Breakable, out var randomPrefabs))
            {
                var Prefabs = new List<BreakableResource.RandomPrefab>(randomPrefabs);
#if SUBNAUTICA_STABLE
                var last = randomPrefabs.GetLast();
                Prefabs.Remove(last);
                __instance.defaultPrefab = last.prefab;
#else
                var last = randomPrefabs.GetLast();
                Prefabs.Remove(last);
                __instance.defaultPrefabTechType = last.prefabTechType;
                __instance.defaultPrefabReference = last.prefabReference;
#endif
                __instance.prefabList = Prefabs;
            }
        }
    }
}
