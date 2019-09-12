using Harmony;
using UnityEngine.UI;

namespace SaveTheSunbeam
{
    [HarmonyPatch(typeof(uGUI_SceneConfirmation))]
    [HarmonyPatch("Start")]
    static class uGUI_SceneConfirmation_Start
    {
        [HarmonyPrefix]
        static void Prefix(uGUI_SceneConfirmation __instance)
        {
            Mod.redSprite = __instance.gameObject.GetComponentInChildren<Image>().sprite;
            Mod.blueSprite = __instance.gameObject.GetComponentsInChildren<Image>()[1].sprite;
        }
    }
}
