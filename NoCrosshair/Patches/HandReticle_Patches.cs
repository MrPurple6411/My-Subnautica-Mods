namespace NoCrosshair.Patches
{
    using HarmonyLib;

    [HarmonyPatch]
    public static class HandReticle_Patches
    {
        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.Awake))]
        public static void Postfix(HandReticle __instance)
        {
            NoCrosshair.icons = __instance._icons;
        }
    }
}