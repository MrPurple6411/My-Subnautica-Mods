using HarmonyLib;

namespace WarpersNoWarping.Patches
{
    [HarmonyPatch(typeof(WarperInspectPlayer), nameof(WarperInspectPlayer.Perform))]
    public class WarperInspectPlayer_Perform
    {
        [HarmonyPrefix]
        public static void Prefix(WarperInspectPlayer __instance)
        {
            __instance.warpOutDistance = 0;
        }
    }
}