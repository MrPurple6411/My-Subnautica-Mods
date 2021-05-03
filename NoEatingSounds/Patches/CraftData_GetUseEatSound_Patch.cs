#if SN1
namespace NoEatingSounds.Patches
{
    using HarmonyLib;

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
#endif