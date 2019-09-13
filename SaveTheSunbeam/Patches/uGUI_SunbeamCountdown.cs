using Harmony;
using System;
using System.Reflection;
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
                Console.WriteLine("Hide");
                AccessTools.Method(typeof(uGUI_SunbeamCountdown), "HideInterface").Invoke(__instance, new object[0]);
                return false;
            }
            Console.WriteLine("Show");
            AccessTools.Method(typeof(uGUI_SunbeamCountdown), "ShowInterface").Invoke(__instance, new object[0]);

            float time = StoryGoalCustomEventHandler.main.endTime - DayNightCycle.main.timePassedAsFloat;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string text = string.Format("{0:D2}:{1:D2}:{2:D2}", Math.Max(timeSpan.Hours, 0), Math.Max(timeSpan.Minutes, 0), Math.Max(timeSpan.Seconds, 0));
            __instance.countdownText.text = text;

            if (!StoryGoalCustomEventHandler.main.gunDisabled)
                __instance.countdownHolder.GetComponentsInChildren<Image>().ForEach(i => i.sprite = Mod.redSprite); 
            else
                __instance.countdownHolder.GetComponentsInChildren<Image>().ForEach(i => i.sprite = Mod.blueSprite);

            if (text == "00:00:00" && StoryGoalCustomEventHandler.main.IsPlayerAtLandingSite())
            {
                AccessTools.Method(typeof(uGUI_SunbeamCountdown), "HideInterface").Invoke(__instance, new object[0]);
                Mod.TriggerSunbeamLanding();
            }

            return false;
        }
    }
}
