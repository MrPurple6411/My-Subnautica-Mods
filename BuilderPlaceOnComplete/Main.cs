using Harmony;
using UnityEngine;

namespace BuilderPlaceOnComplete
{
    [HarmonyPatch(typeof(BuilderTool), nameof(BuilderTool.HandleInput))]
    public class BuilderTool_HandleInput
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            TechType techType = PDAScanner.scanTarget.techType;
            if (Input.GetMouseButtonDown(2) && CrafterLogic.IsCraftRecipeUnlocked(techType))
            {
#if SUBNAUTICA
                GameObject prefab = CraftData.GetPrefabForTechType(techType);
                Builder.Begin(prefab);
#elif BELOWZERO
                Builder.Begin(techType);
#endif
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Constructable), nameof(Constructable.Construct))]
    public class Constructable_Construct
    {
        [HarmonyPostfix]
        public static void Postfix(Constructable __instance)
        {
            if (__instance.constructed)
            {
#if SUBNAUTICA
                Builder.Begin(CraftData.GetPrefabForTechType(CraftData.GetTechType(__instance.gameObject)));
#elif BELOWZERO
                Builder.Begin(CraftData.GetTechType(__instance.gameObject));
#endif
            }
        }
    }
}