using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace NoEatingSounds
{
    [HarmonyPatch(typeof(CraftData), "GetUseEatSound")]
    internal class CraftData_GetUseEatSound_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(ref string __result)
        {
            __result = "";
        }
    }
}