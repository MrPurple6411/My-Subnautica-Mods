using Harmony;
using System;
using UnityEngine.UI;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(uGUI_SunbeamCountdown))]
    [HarmonyPatch("UpdateInterface")]
    static class uGUI_SunbeamCountdown_UpdateInterface
    {
        [HarmonyPrefix]
        static bool Prefix(uGUI_SunbeamCountdown __instance)
        {
            if (!StoryGoalCustomEventHandler.main) return false;
            if (!StoryGoalCustomEventHandler.main.countdownActive)
            {
                typeof(uGUI_SunbeamCountdown).GetMethod("HideInterface").Invoke(__instance, new object[0]);
                return false;
            }
            typeof(uGUI_SunbeamCountdown).GetMethod("ShowInterface").Invoke(__instance, new object[0]);

            float time = StoryGoalCustomEventHandler.main.endTime - DayNightCycle.main.timePassedAsFloat;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            __instance.countdownText.text = text;

            if (!StoryGoalCustomEventHandler.main.gunDisabled)
                __instance.countdownHolder.GetComponentsInChildren<Image>().ForEach(i => i.sprite = Mod.redSprite); 
            else
                __instance.countdownHolder.GetComponentsInChildren<Image>().ForEach(i => i.sprite = Mod.blueSprite); 

            return false;
        }
    }
}
