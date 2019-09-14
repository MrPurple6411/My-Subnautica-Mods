using Harmony;
using System;
using System.Collections;
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

            names.text = "\n\nMrPurple6411\nAlexejheroYTB\nNairda\nSeraphim\nJgirard1733\n\n\n" + names.text;
            roles.text = "\n\nMod Developer\nMod Developer\nModeler\nTranscript\nVoice Actor\n\n\n" + roles.text;
            main.text = "<b>Alternate Sunbeam Event</b>\n\n\n\n\n\n\n\n\n" + main.text;
            __instance.scrollMaxValue += 200;
            __instance.secondsUntilScrollComplete -= 5.5f;
        }
    }

    [HarmonyPatch(typeof(EndCreditsManager))]
    [HarmonyPatch("ReturnToMainMenu")]
    static class EndCreditsManager_ReturnToMainMenu
    {
        [HarmonyPostfix]
        static IEnumerator Postfix(IEnumerator iterator, EndCreditsManager __instance)
        {
            bool found = false;
            while (iterator.MoveNext())
            {
                if (iterator.Current is WaitForSeconds && !found)
                {
                    found = true;
                    yield return new WaitForSeconds(__instance.secondsUntilScrollComplete - 3f + 5.5f);
                }
                else
                {
                    yield return iterator.Current;
                }
            }
        }
    }
}
