using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace SaveTheSunbeam
{
    [HarmonyPatch(typeof(uGUI_SceneConfirmation))]
    [HarmonyPatch("Start")]
    static class uGUI_SceneConfirmation_Start
    {
        public static Sprite redSprite;
        public static Sprite blueSprite;

        [HarmonyPrefix]
        static void Prefix(uGUI_SceneConfirmation __instance)
        {
            redSprite = __instance.gameObject.GetComponentInChildren<Image>().sprite;
            blueSprite = __instance.gameObject.GetComponentsInChildren<Image>()[1].sprite;
        }
    }
}
