using HarmonyLib;

namespace ImprovedPowerNetwork.Patches
{
    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.UpdatePowerState))]
    public static class PowerRelay_UpdatePowerState_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(PowerRelay __instance)
        {
            if(__instance.outboundRelay != null && __instance.GetMaxPower() == 0)
            {
                __instance.UpdateConnection();
            }
        }
    }
}
