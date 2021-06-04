namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.GetMaxPower))]
    public static class PowerRelay_GetMaxPower_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance, ref float __result)
        {
            var powerInterface = __instance.inboundPowerSources.Find((x) => x is BaseInboundRelay or OtherConnectionRelay);
            var powerControl = powerInterface switch
            {
                BaseInboundRelay baseConnectionRelay => baseConnectionRelay.gameObject.GetComponent<PowerControl>(),
                OtherConnectionRelay otherConnectionRelay => otherConnectionRelay.gameObject.GetComponent<PowerControl>(),
                _ => null
            };

            if(powerControl is null) return;

            var endRelay = powerControl.Relay.GetEndpoint();
            var endPower = endRelay.GetMaxPower();
            var powerHere = powerInterface.GetMaxPower();

            if(endPower > powerHere)
            {
                __result += endPower - powerHere;
            }
        }
    }
}
