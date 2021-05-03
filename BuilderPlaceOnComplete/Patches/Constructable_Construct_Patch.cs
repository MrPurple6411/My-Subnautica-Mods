#if SN1
namespace BuilderPlaceOnComplete.Patches
{
    using HarmonyLib;
    using System.Collections;
    using UnityEngine;
    using UWE;

    [HarmonyPatch(typeof(Constructable), nameof(Constructable.Construct))]
    public class Constructable_Construct_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Constructable __instance)
        {
            if(__instance.constructed)
            {
                CoroutineHost.StartCoroutine(InitializeBuilder(CraftData.GetTechType(__instance.gameObject)));
            }
        }

        private static IEnumerator InitializeBuilder(TechType techType)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;

            GameObject prefab = task.GetResult();

            Builder.Begin(prefab);
            yield break;
        }
    }
}
#endif