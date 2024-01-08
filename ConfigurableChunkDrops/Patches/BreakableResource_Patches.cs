namespace ConfigurableChunkDrops.Patches;

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

[HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
public static class BreakableResource_ChooseRandomResource
{
    public static Dictionary<TechType, List<BreakableResource.RandomPrefab>> prefabs = new();

    [HarmonyPrefix, HarmonyPriority(Priority.Last)]
    public static void Prefix(BreakableResource __instance, bool __runOriginal)
    {
		if(!__runOriginal)
			return;

        var Breakable = CraftData.GetTechType(__instance.gameObject);

        if(prefabs.TryGetValue(Breakable, out var randomPrefabs))
        {
            List<BreakableResource.RandomPrefab> Prefabs = new List<BreakableResource.RandomPrefab>(randomPrefabs).OrderBy(x => x.chance).ToList();
            var last = randomPrefabs.GetLast();
            Prefabs.Remove(last);
            __instance.defaultPrefabTechType = last.prefabTechType;
            __instance.defaultPrefabReference = last.prefabReference;
            __instance.prefabList = Prefabs;
        }
    }
}
