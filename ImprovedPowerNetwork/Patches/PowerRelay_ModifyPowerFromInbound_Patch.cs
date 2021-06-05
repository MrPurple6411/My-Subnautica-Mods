namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.ModifyPowerFromInbound))]
    public static class PowerRelay_ModifyPowerFromInbound_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance, ref bool __result, ref float amount, ref float modified)
        {
            if (__result) return;
            var powerInterface = __instance.inboundPowerSources.Find((x) => x is BaseInboundRelay or OtherConnectionRelay);

            var powerControl = powerInterface switch
            {
                BaseInboundRelay baseConnectionRelay => baseConnectionRelay.gameObject.GetComponent<PowerControl>(),
                OtherConnectionRelay otherConnectionRelay => otherConnectionRelay.gameObject.GetComponent<PowerControl>(),
                _ => null
            };

            if(powerControl is null) return;
            var endRelay = powerControl.Relay.GetEndpoint();

            if (!(endRelay.GetMaxPower() > powerInterface.GetMaxPower())) return;
            __result = endRelay.ModifyPowerFromInbound(amount, out var newModified);
            modified += newModified;
        }
    }
}
