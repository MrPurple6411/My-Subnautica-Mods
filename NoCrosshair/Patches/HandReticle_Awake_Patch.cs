using System.Collections.Generic;
using HarmonyLib;

namespace NoCrosshair.Patches
{
    [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.Awake))]
    public static class HandReticle_Awake_Patch
    {
        public static void Postfix(HandReticle __instance)
        {
            NoCrosshair.icons = __instance._icons;
            NoCrosshair.ChangeCrosshair(false);
        }
    }
}