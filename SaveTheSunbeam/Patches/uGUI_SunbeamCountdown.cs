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
            {
                // Make the default sunbeam countdown red
                __instance.countdownHolder.GetComponentsInChildren<Image>().ForEach(i => i.sprite = Mod.redSprite); 
            }
            else
            {
                // Make the countdown blue if the gun is disabled
                __instance.countdownHolder.GetComponentsInChildren<Image>().ForEach(i => i.sprite = Mod.blueSprite); 
            }
        }
    }
}
