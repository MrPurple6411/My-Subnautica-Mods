using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImprovedPowerNetwork
{
    internal class PowerControl : MonoBehaviour, IHandTarget 
    {
        public PowerRelay powerRelay;
        public Constructable constructable;
        public List<BaseInboundRelay> baseConnectionRelays = new List<BaseInboundRelay>();
        public List<OtherConnectionRelay> otherConnectionRelays = new List<OtherConnectionRelay>();
        public bool baseConnectionsEnabled = false;
        public bool otherConnectionsEnabled = false;

        public void OnHandClick(GUIHand hand)
        {
            if (!hand.IsTool())
            {
                powerRelay.dontConnectToRelays = !powerRelay.dontConnectToRelays;
                RefreshNetwork();
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!hand.IsTool())
            {
#if SN1
                HandReticle.main.SetInteractText($"Max Power {(int)powerRelay.GetMaxPower()}, Current Power: {(int)powerRelay.GetPower()}\nMainConnections: {!powerRelay.dontConnectToRelays}, BaseConnections: {!baseConnectionsEnabled}, Other Connections: {!otherConnectionsEnabled}", "LeftHand: Full Enable/Disable\nAltTool Key (F): BaseConnections (Purple)\nDeconstruct Key (Q): Other Connections (Green)", false, false, HandReticle.Hand.None);
#elif BZ
                HandReticle.main.SetText(HandReticle.TextType.Hand, $"Max Power {(int)powerRelay.GetMaxPower()}, Current Power: {(int)powerRelay.GetPower()}\nMainConnections: {!powerRelay.dontConnectToRelays}, BaseConnections: {!baseConnectionsEnabled}, Other Connections: {!otherConnectionsEnabled}", false);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "LeftHand: Full Enable/Disable\nAltTool Key (F): BaseConnections (Purple)\nDeconstruct Key (Q): Other Connections (Green)", false);
#endif
                if (GameInput.GetButtonDown(GameInput.Button.AltTool))
                {
                    baseConnectionsEnabled = !baseConnectionsEnabled;
                    baseConnectionRelays.ForEach((x)=> { 
                        x.dontConnectToRelays = baseConnectionsEnabled;
                        x.DisconnectFromRelay();
                    });
                    RefreshNetwork();
                }

                if (GameInput.GetButtonDown(GameInput.Button.Deconstruct))
                {
                    otherConnectionsEnabled = !otherConnectionsEnabled;
                    otherConnectionRelays.ForEach((x) => {
                        x.dontConnectToRelays = otherConnectionsEnabled;
                        x.DisconnectFromRelay();
                    });
                    RefreshNetwork();
                }
            }
        }

        public void RefreshNetwork()
        {
            foreach(PowerRelay relay in PowerRelay.relayList)
            {
                relay.DisconnectFromRelay();
            }
            PowerRelay.MarkRelaySystemDirty();
        }

        public void Start()
        {
            powerRelay = gameObject.GetComponent<PowerRelay>();
            constructable = gameObject.GetComponent<Constructable>();
        }

        public void LateUpdate()
        {
            bool openBaseConnector = false;
            bool openOtherConnector = false;

            baseConnectionRelays.ForEach((x) => { if (x.outboundRelay == null) openBaseConnector = true; });
            otherConnectionRelays.ForEach((x) => { if (x.outboundRelay == null) openOtherConnector = true; });

            if (!openBaseConnector)
            {
                BaseInboundRelay.AddNewBaseConnectionRelay(powerRelay, this);
            }

            if (!openOtherConnector)
            {
                OtherConnectionRelay.AddNewOtherConnectionRelay(powerRelay, this);
            }
        }

        public void OnDisable()
        {
            if (!constructable?.constructed ?? false)
            {
                if (powerRelay?.outboundRelay != null)
                {
                    powerRelay.outboundRelay.RemoveInboundPower(powerRelay);
                    powerRelay.outboundRelay = null;
                    powerRelay.powerFX.target = null;
                    GameObject.Destroy(powerRelay.powerFX.vfxEffectObject);
                }

                foreach (PowerRelay powerRelay in new List<PowerRelay>().Concat(baseConnectionRelays).Concat(otherConnectionRelays))
                {
                    if (powerRelay.outboundRelay != null)
                    {
                        powerRelay.outboundRelay.RemoveInboundPower(powerRelay);
                        powerRelay.outboundRelay = null;
                        powerRelay.powerFX.target = null;
                        GameObject.Destroy(powerRelay.powerFX.vfxEffectObject);
                    }
                }
                RefreshNetwork();
            }
        }

        public void OnDestroy()
        {
            baseConnectionRelays.ForEach((x) => x.DisconnectFromRelay());
            otherConnectionRelays.ForEach((x) => x.DisconnectFromRelay());
        }
    }
}
