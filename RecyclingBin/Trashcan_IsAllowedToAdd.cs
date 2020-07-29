using HarmonyLib;

namespace RecyclingBin
{
    [HarmonyPatch(typeof(Trashcan), nameof(Trashcan.IsAllowedToAdd))]
    internal class Trashcan_IsAllowedToAdd
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}