using System;
using Harmony;
using UnityEngine;

namespace NoOxygenWarnings
{


    [HarmonyPatch(typeof(LowOxygenAlert))]
    [HarmonyPatch("Update")]
    internal class AlertBreaker
    {
        
        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            return false;
        }



    }
}
