using Harmony;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                    __instance.sunbeamEvent1.Trigger();
                    key = "nope";
                }
            }
            if (string.Equals(key, "SunbeamCheckPlayerRange", StringComparison.OrdinalIgnoreCase))
            {
                if (__instance.gunDisabled)
                {
                    MonoBehaviour main = __instance;
                    __instance.countdownActive = false;
                    main.Invoke("StartSunbeamShootdownFX", 26f);
                    if (VFXSunbeam.main != null)
                    {
                        VFXSunbeam.main.PlaySFX();
                        VFXSunbeam.main.PlaySequence();
                    }
                    else
                    {
                        UWE.Utils.LogReport("VFXSunbeam.main can not be found", null);
                    }
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
                SceneManager.LoadSceneAsync("EndCreditsSceneCleaner", LoadSceneMode.Single);
                return false;
            }
            return true;
        }
    }
}
