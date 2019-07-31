using System;
using Harmony;
using UnityEngine;

namespace MrPurple
{


    [HarmonyPatch(typeof(HintSwimToSurface))]
    [HarmonyPatch("ShouldShowWarning")]
    internal class WarningsBreaker
    {
        
        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            return false;
        }



    }
}
