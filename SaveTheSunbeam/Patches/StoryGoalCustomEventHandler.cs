using Harmony;
using Story;
using System;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(StoryGoalCustomEventHandler))]
    [HarmonyPatch("NotifyGoalComplete")]
    static class StoryGoalCustomEventHandler_NotifyGoalComplete
    {
        [HarmonyPrefix]
        static void Prefix(StoryGoalCustomEventHandler __instance, ref string key)
        {
            if (string.Equals(key, "RadioSunbeamStart", StringComparison.OrdinalIgnoreCase))
            {
                if (__instance.gunDisabled)
                {
                    // Prevent the default Sunbeam subtitles from showing
                    __instance.sunbeamEvent1.Trigger();
                    key = "nope"; 
                }
            }
            if (string.Equals(key, "SunbeamCheckPlayerRange", StringComparison.OrdinalIgnoreCase))
            {
                if (__instance.gunDisabled)
                {
                    ErrorMessage.AddDebug("SUNBEAM EVENT TRIGGERED!");
                    __instance.countdownActive = false;
                    __instance.Invoke("StartSunbeamShootdownFX", 26f);
                    if (VFXSunbeam.main != null)
                    {
                        VFXSunbeam.main.PlaySFX();
                        VFXSunbeam.main.PlaySequence();
                    }
                    else
                    {
                        UWE.Utils.LogReport("VFXSunbeam.main can not be found", null);
                    }
                    // Prevent the default Sunbeam animation from being played
                    key = "nope"; 
                }
            }
        }
    }

    [HarmonyPatch(typeof(StoryGoalCustomEventHandler))]
    [HarmonyPatch("StartSunbeamShootdownFX")]
    static class StoryGoalCustomEventHandler_StartSunbeamShootdownFX
    {
        [HarmonyPrefix]
        static bool Prefix(StoryGoalCustomEventHandler __instance)
        {
            if (StoryGoalCustomEventHandler.main.gunDisabled && __instance != null)
            {
                //SceneManager.LoadSceneAsync("EndCreditsSceneCleaner", LoadSceneMode.Single);
                return false;
            }
            return true;
        }
    }
}
