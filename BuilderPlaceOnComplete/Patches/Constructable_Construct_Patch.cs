namespace BuilderPlaceOnComplete.Patches
{
    using HarmonyLib;
    using System.Collections;
    using UWE;

    [HarmonyPatch(typeof(Constructable), nameof(Constructable.Construct))]
    public class Constructable_Construct_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Constructable __instance)
        {
            if(__instance.constructed)
                CoroutineHost.StartCoroutine(InitializeBuilder(CraftData.GetTechType(__instance.gameObject)));
        }

        private static IEnumerator InitializeBuilder(TechType techType)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;

            var prefab = task.GetResult();
#if SN1
            Builder.Begin(prefab);
#elif BZ
            Builder.Begin(techType, prefab);
#endif
        }
    }
}