using UnityEngine;

namespace ImprovedPowerNetwork
{
    public class OtherConnectionRelay : PowerRelay
    {
        internal static void AddNewOtherConnectionRelay(PowerRelay originalRelay, PowerControl powerControl)
        {
            OtherConnectionRelay additionalRelay = originalRelay.gameObject.AddComponent<OtherConnectionRelay>();
            additionalRelay.dontConnectToRelays = powerControl.otherConnectionsEnabled;
            additionalRelay.maxOutboundDistance = 15;
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

            powerControl.otherConnectionRelays.Add(additionalRelay);
        }

        public class OtherRelayPowerFX : PowerFX { }
    }
}