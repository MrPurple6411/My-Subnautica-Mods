namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.GetPowerFromInbound))]
    public static class PowerRelay_GetPowerFromInBound_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance, ref float __result)
        {
            var powerInterface = __instance.inboundPowerSources.Find((x) => x is BaseInboundRelay or OtherConnectionRelay);

            var powerControl = powerInterface switch
            {
                BaseInboundRelay baseConnectionRelay => baseConnectionRelay.gameObject.GetComponent<PowerControl>(),
                OtherConnectionRelay otherConnectionRelay =>
                    otherConnectionRelay.gameObject.GetComponent<PowerControl>(),
                _ => null
            };

            if(powerControl == null) return;

            var endRelay = powerControl.Relay.GetEndpoint();
            var endPower = endRelay.GetPower();
            var powerHere = powerInterface.GetPower();

            if(endPower > powerHere)
            {
                __result += endPower - powerHere;
            }
        }
    }
}
