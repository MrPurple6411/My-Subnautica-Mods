using HarmonyLib;
using System.Collections;
using UnityEngine;
using UWE;

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
                CoroutineHost.StartCoroutine(InitializeBuilder(techType));
                return false;
            }
            return true;
        }

        private static IEnumerator InitializeBuilder(TechType techType)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;

            GameObject gameObject = GameObject.Instantiate(task.GetResult());
            Builder.Begin(gameObject);
            yield break;
        }
    }
}
