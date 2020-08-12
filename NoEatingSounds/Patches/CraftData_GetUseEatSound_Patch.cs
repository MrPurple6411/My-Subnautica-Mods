using HarmonyLib;

namespace NoEatingSounds.Patches
{
    [HarmonyPatch(typeof(CraftData), nameof(CraftData.GetUseEatSound))]
    internal class Data_GetUseEatSound_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(ref string __result)
        {
            __result = "";
        }
    }
}