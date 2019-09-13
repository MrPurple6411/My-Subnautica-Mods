using Harmony;
using UnityEngine;
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
            var texts = GameObject.FindObjectsOfType<Text>();
            var names = texts[0];
            var roles = texts[1];
            var main = texts[2];

            names.text = "\n\nMrPurple6411\nAlexejheroYTB\n\n\n" + names.text;
            roles.text = "\n\nMod Developer\nMod Developer\n\n\n" + roles.text;
            main.text = "<b>Alternate Sunbeam Event</b>\n\n\n\n\n\n" + main.text;
        }
    }
}
