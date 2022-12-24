namespace WarpersNoWarping.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(WarpOut), nameof(WarpOut.Evaluate))]
    public class WarpOut_Evaluate
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result)
        {
            __result = 0f;
            return false;
        }
    }
}