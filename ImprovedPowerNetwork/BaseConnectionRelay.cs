namespace ImprovedPowerNetwork
{
    public class BaseConnectionRelay : PowerRelay
    {
        internal static void EnsureBaseConnectionRelay(PowerRelay originalRelay)
        {
            if (originalRelay.gameObject.GetComponent<BaseConnectionRelay>() == null)
            {
                BaseConnectionRelay additionalRelay = originalRelay.gameObject.AddComponent<BaseConnectionRelay>();
                additionalRelay.dontConnectToRelays = originalRelay.dontConnectToRelays;
                additionalRelay.maxOutboundDistance = originalRelay.maxOutboundDistance;
                additionalRelay.constructable = originalRelay.constructable;

                if (originalRelay.powerFX != null && originalRelay.powerFX.vfxPrefab != null)
                {
                    BaseConnectionRelayPowerFX powerFX = originalRelay.gameObject.AddComponent<BaseConnectionRelayPowerFX>();
                    powerFX.attachPoint = originalRelay.powerFX.attachPoint;
                    powerFX.vfxPrefab = originalRelay.powerFX.vfxPrefab;
                    additionalRelay.powerFX = powerFX;
                }
                additionalRelay.AddInboundPower(originalRelay);
            }
        }

        public class BaseConnectionRelayPowerFX : PowerFX { }
    }
}