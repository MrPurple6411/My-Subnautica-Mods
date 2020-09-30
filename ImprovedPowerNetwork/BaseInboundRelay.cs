using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ImprovedPowerNetwork
{
    public class BaseInboundRelay : PowerRelay
    {
        internal static void AddNewBaseConnectionRelay(PowerRelay originalRelay, PowerControl powerControl)
        {
            BaseInboundRelay additionalRelay = originalRelay.gameObject.AddComponent<BaseInboundRelay>();
            additionalRelay.dontConnectToRelays = powerControl.baseConnectionsDisabled;
            additionalRelay.maxOutboundDistance = 15;
            additionalRelay.constructable = originalRelay.constructable;

            if (originalRelay.powerFX != null && originalRelay.powerFX.vfxPrefab != null)
            {
                BaseInboundRelayPowerFX powerFX = originalRelay.gameObject.AddComponent<BaseInboundRelayPowerFX>();
                powerFX.attachPoint = originalRelay.powerFX.attachPoint;
                powerFX.vfxPrefab = GameObject.Instantiate(originalRelay.powerFX.vfxPrefab);
                powerFX.vfxPrefab.SetActive(false);
                powerFX.vfxPrefab.GetComponent<LineRenderer>().material.SetColor(ShaderPropertyID._Color, Color.magenta);


                additionalRelay.powerFX = powerFX;
            }
            additionalRelay.AddInboundPower(originalRelay);

            powerControl.baseConnectionRelays.Add(additionalRelay);
        }

        public class BaseInboundRelayPowerFX : PowerFX { }
    }
}