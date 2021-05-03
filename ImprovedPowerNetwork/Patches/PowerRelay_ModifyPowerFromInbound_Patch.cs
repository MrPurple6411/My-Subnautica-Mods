namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.ModifyPowerFromInbound))]
    public static class PowerRelay_ModifyPowerFromInbound_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance, ref bool __result, ref float amount, ref float modified)
        {
            if(!__result)
            {
                IPowerInterface powerInterface = __instance.inboundPowerSources.Find((x) => x is BaseInboundRelay || x is OtherConnectionRelay);

                if(powerInterface != null)
                {
                    PowerControl powerControl = null;
                    switch(powerInterface)
                    {
                        case BaseInboundRelay baseConnectionRelay:
                            powerControl = baseConnectionRelay.gameObject.GetComponent<PowerControl>();
                            break;
                        case OtherConnectionRelay otherConnectionRelay:
                            powerControl = otherConnectionRelay.gameObject.GetComponent<PowerControl>();
                            break;
                    }

                    PowerRelay endRelay = powerControl.powerRelay.GetEndpoint();

                    if(endRelay.GetMaxPower() > powerInterface.GetMaxPower())
                    {
                        __result = endRelay.ModifyPowerFromInbound(amount, out float newModified);
                        modified += newModified;
                    }
                }
            }
        }
    }
}
