using Harmony;
using UnityEngine.UI;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(uGUI_SunbeamCountdown))]
    [HarmonyPatch("UpdateInterface")]
    static class uGUI_SunbeamCountdown_UpdateInterface
    {
        [HarmonyPostfix]
        static void Postfix(uGUI_SunbeamCountdown __instance)
        {
            if (!StoryGoalCustomEventHandler.main.gunDisabled)
                __instance.countdownHolder.GetComponentsInChildren<Image>().ForEach(i => i.sprite = Mod.redSprite); 
            else
                __instance.countdownHolder.GetComponentsInChildren<Image>().ForEach(i => i.sprite = Mod.blueSprite); 
        }
    }
}
