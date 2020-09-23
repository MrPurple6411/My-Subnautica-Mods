using UnityEngine;

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
                    powerFX.vfxPrefab = GameObject.Instantiate(originalRelay.powerFX.vfxPrefab);
                    powerFX.vfxPrefab.SetActive(false);
                    powerFX.vfxPrefab.GetComponent<LineRenderer>().material.SetColor(ShaderPropertyID._Color, Color.green);

                    additionalRelay.powerFX = powerFX;
                }
                additionalRelay.AddInboundPower(originalRelay);
            }
        }
        public class OtherRelayPowerFX : PowerFX { }
    }
}