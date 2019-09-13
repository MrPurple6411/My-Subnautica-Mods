using Harmony;
using System;
using UnityEngine.UI;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(EndCreditsManager))]
    [HarmonyPatch("Start")]
    static class EndCreditsManager_Start
    {
        [HarmonyPrefix]
        static void Prefix(EndCreditsManager __instance)
        {
            Console.WriteLine(__instance.creditsText.gameObject.GetComponentInChildren<Text>().text);
        }
    }
}
