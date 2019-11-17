using Harmony;
using System;
using System.Reflection;
using UnityEngine;

namespace Eternal_Sunshine
{
    public class Main
    {
        public static void Load()
        {
            try
            {
                HarmonyInstance.Create("MrPurple6411.Eternal_Sunshine").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }


    [HarmonyPatch(typeof(DayNightCycle))]
    [HarmonyPatch("GetDayNightCycleTime")]
    internal class Builder_Update_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(DayNightCycle __instance)
        {
            __instance.sunRiseTime = -1000.0f;
            __instance.sunSetTime = 1000.0f;

            return true;
        }
    }
}
