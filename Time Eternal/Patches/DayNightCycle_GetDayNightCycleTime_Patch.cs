namespace Time_Eternal.Patches;

using HarmonyLib;

[HarmonyPatch(typeof(DayNightCycle), nameof(DayNightCycle.GetDayNightCycleTime))]
internal class DayNightCycle_GetDayNightCycleTime_Patch
{
    [HarmonyPrefix]
    private static bool Prefix(DayNightCycle __instance)
    {
        switch(Main.SMLConfig.freezeTimeChoice)
        {
            case 1:
                //always day
                __instance.sunRiseTime = -1000.0f;
                __instance.sunSetTime = 1000.0f;
                return true;
            case 2:
                //always night
                __instance.sunRiseTime = 1000.0f;
                __instance.sunSetTime = -1000.0f;
                return true;
            default:
                //9pm to 3am game default
                __instance.sunRiseTime = 0.125f;
                __instance.sunSetTime = 0.875f;
                return true;
        }
    }

    //[HarmonyPostfix]
    //private static void Postfix(DayNightCycle __instance, ref float __result)
    //{
    //    Main.logSource.LogDebug($"DayNightCycleTime: {__result}");
    //}
}