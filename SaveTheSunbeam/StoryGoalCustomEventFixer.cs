using System;
using System.Collections;
using Gendarme;
using mset;
using Story;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;

namespace SaveTheSunbeam
{
    [HarmonyPatch(typeof(StoryGoalCustomEventHandler))]
    [HarmonyPatch("NotifyGoalComplete")]
    internal class DisableGunFixer
    {
        [HarmonyPrefix]
        public static void Prefix(StoryGoalCustomEventHandler __instance, ref string key)
        {
            if (string.Equals(key, "RadioSunbeamStart", StringComparison.OrdinalIgnoreCase))
            {
                if (__instance.gunDisabled)
                {
                    __instance.sunbeamEvent1.Trigger();
                    key = "nope";
                    Console.WriteLine("MrPurple6411");
                }
            }
            if (string.Equals(key, "SunbeamCheckPlayerRange", StringComparison.OrdinalIgnoreCase))
            {
                if (__instance.gunDisabled)
                {
                    MonoBehaviour main = __instance;
                    __instance.countdownActive = false;
                    main.Invoke("StartSunbeamShootdownFX", 12f);
                    if (VFXSunbeam.main != null)
                    {
                        VFXSunbeam.main.PlaySFX();
                    }
                    else
                    {
                        UWE.Utils.LogReport("VFXSunbeam.main can not be found", null);
                    }
                    key = "nope";
                    Console.WriteLine("MrPurple6411");
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
