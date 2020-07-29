using System.Collections.Generic;
using HarmonyLib;

namespace NoCrosshair
{
    [HarmonyPatch(typeof(HandReticle), "Awake")]
    public static class HandReticle_Awake_Patch
    {
        public static void Postfix(HandReticle __instance)
        {
            NoCrosshair.icons = AccessTools.Field(typeof(HandReticle), "_icons").GetValue(__instance) as Dictionary<HandReticle.IconType, uGUI_HandReticleIcon>;
            NoCrosshair.ChangeCrosshair(false);
        }
    }
}