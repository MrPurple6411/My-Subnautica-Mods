using HarmonyLib;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Builder), nameof(Builder.ValidateOutdoor))]
    internal class Builder_ValidateOutdoor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
}
