namespace ImprovedPowerNetwork
{
    public class OtherConnectionRelay : PowerRelay
    {
        internal static void EnsureOtherConnectionRelay(PowerRelay originalRelay)
        {
            if (originalRelay.gameObject.GetComponent<OtherConnectionRelay>() == null)
            {
                OtherConnectionRelay additionalRelay = originalRelay.gameObject.AddComponent<OtherConnectionRelay>();
                additionalRelay.dontConnectToRelays = originalRelay.dontConnectToRelays;
                additionalRelay.maxOutboundDistance = originalRelay.maxOutboundDistance;
                additionalRelay.constructable = originalRelay.constructable;

                if (originalRelay.powerFX != null && originalRelay.powerFX.vfxPrefab != null)
                {
                    OtherRelayPowerFX powerFX = originalRelay.gameObject.AddComponent<OtherRelayPowerFX>();
                    powerFX.attachPoint = originalRelay.powerFX.attachPoint;
                    powerFX.vfxPrefab = originalRelay.powerFX.vfxPrefab;
                    additionalRelay.powerFX = powerFX;
                }
                additionalRelay.AddInboundPower(originalRelay);
            }
        }
        public class OtherRelayPowerFX : PowerFX { }
    }
}