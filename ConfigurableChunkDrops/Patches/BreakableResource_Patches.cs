using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace ConfigurableChunkDrops.Patches
{
    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    public static class BreakableResource_ChooseRandomResource
    {

        public static Dictionary<TechType, List<BreakableResource.RandomPrefab>> prefabs = new Dictionary<TechType, List<BreakableResource.RandomPrefab>>();


        [HarmonyPrefix]
        public static void Prefix(BreakableResource __instance)
        {
            TechType Breakable = CraftData.GetTechType(__instance.gameObject);

            if (prefabs.TryGetValue(Breakable, out List<BreakableResource.RandomPrefab> randomPrefabs))
            {
                __instance.prefabList = randomPrefabs;
                return;
            }
            


        }

    }
}
