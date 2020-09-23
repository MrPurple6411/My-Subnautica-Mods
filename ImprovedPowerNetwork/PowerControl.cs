using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace ImprovedPowerNetwork
{
    internal class PowerControl : MonoBehaviour, IHandTarget 
    {
        private PowerRelay powerRelay;
        private BaseConnectionRelay baseConnectionRelay;
        private OtherConnectionRelay otherConnectionRelay;

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
                HandReticle.main.SetInteractText($"MainConnections: {!powerRelay.dontConnectToRelays}, BaseConnections: {!baseConnectionRelay.dontConnectToRelays}, Other Connections: {!otherConnectionRelay.dontConnectToRelays}", "LeftHand: Full Enable/Disable\nAltTool Key (F): BaseConnections (Purple)\nDeconstruct Key (Q): Other Connections (Green)", false, false, HandReticle.Hand.None);

                if (GameInput.GetButtonDown(GameInput.Button.AltTool))
                {
                    baseConnectionRelay.dontConnectToRelays = !baseConnectionRelay.dontConnectToRelays;
                    RefreshNetwork();
                }

                if (GameInput.GetButtonDown(GameInput.Button.Deconstruct))
                {
                    otherConnectionRelay.dontConnectToRelays = !otherConnectionRelay.dontConnectToRelays;
                    RefreshNetwork();
                }
            }
        }

        private void RefreshNetwork()
        {
            foreach(PowerRelay relay in PowerRelay.relayList)
            {
                relay.DisconnectFromRelay();
            }
        }

        public void Start()
        {
            powerRelay = gameObject.GetComponent<PowerRelay>();
            baseConnectionRelay = gameObject.GetComponent<BaseConnectionRelay>();
            otherConnectionRelay = gameObject.GetComponent<OtherConnectionRelay>();

        }
    }
}
