using HarmonyLib;

namespace BuilderPlaceOnComplete
{
    [HarmonyPatch(typeof(Constructable), nameof(Constructable.Construct))]
    public class Constructable_Construct_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Constructable __instance)
        {
            if (__instance.constructed)
            {
                Builder.Begin(CraftData.GetPrefabForTechType(CraftData.GetTechType(__instance.gameObject)));
            }
        }
    }
}