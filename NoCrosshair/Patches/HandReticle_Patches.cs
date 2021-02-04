using HarmonyLib;

namespace NoCrosshair.Patches
{
    [HarmonyPatch]
    public static class HandReticle_Patches
    {
        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.LateUpdate))]
        public static void Postfix(HandReticle __instance)
        {
            if(NoCrosshair.icons is null)
                NoCrosshair.icons = __instance._icons;
        }
    }
}