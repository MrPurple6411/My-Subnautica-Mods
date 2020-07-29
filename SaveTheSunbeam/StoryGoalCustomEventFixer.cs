#if SUBNAUTICA

using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveTheSunbeam
{
    [HarmonyPatch(typeof(StoryGoalCustomEventHandler))]
    [HarmonyPatch("NotifyGoalComplete")]
    internal class DisableGunFixer
    {
        [HarmonyPrefix]
        public static void Prefix(StoryGoalCustomEventHandler __instance, ref string key)
        {
            foreach (StoryGoalCustomEventHandler.SunbeamGoal sunbeamGoal in __instance.sunbeamGoals)
            {
                if (string.Equals(key, sunbeamGoal.trigger, StringComparison.OrdinalIgnoreCase))
                {
                    if (__instance.gunDisabled)
                    {
                        sunbeamGoal.Trigger();
                        key = "nope";
                    }
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
                        Debug.LogError("VFXSunbeam.main can not be found", null);
                    }
                    key = "nope";
                }
            }
        }
    }

    [HarmonyPatch(typeof(StoryGoalCustomEventHandler))]
    [HarmonyPatch("StartSunbeamShootdownFX")]
    internal class NotifyGoalCompleteFixer
    {
        [HarmonyPrefix]
        public static bool Prefix(StoryGoalCustomEventHandler __instance)
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

#endif