using System;
using Harmony;
using UnityEngine;

namespace NoOxygenWarnings
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
