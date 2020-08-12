using HarmonyLib;
using UnityEngine;

namespace BuilderPlaceOnComplete.Patches
{
    [HarmonyPatch(typeof(BuilderTool), "HandleInput")]
    public class BuilderTool_HandleInput_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            TechType techType = PDAScanner.scanTarget.techType;
            if (Input.GetMouseButtonDown(2) && CrafterLogic.IsCraftRecipeUnlocked(techType))
            {
                GameObject prefab = CraftData.GetPrefabForTechType(techType);
                Builder.Begin(prefab);
                return false;
            }
            return true;
        }
    }
}
