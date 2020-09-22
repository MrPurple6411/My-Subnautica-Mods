using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImprovedPowerNetwork
{
    internal class PowerControl : MonoBehaviour, IHandTarget 
    {
        private PowerRelay powerRelay;
        private BaseConnectionRelay baseConnectionRelay;
        private OtherConnectionRelay otherConnectionRelay;

        public void OnHandClick(GUIHand hand)
        {
            powerRelay.dontConnectToRelays = !powerRelay.dontConnectToRelays;
            PowerRelay.MarkRelaySystemDirty();
        }

        public void OnHandHover(GUIHand hand)
        {
            if (GameInput.GetButtonDown(GameInput.Button.AltTool))
            {
                baseConnectionRelay.dontConnectToRelays = !baseConnectionRelay.dontConnectToRelays;
                PowerRelay.MarkRelaySystemDirty();
            }

            if (GameInput.GetButtonDown(GameInput.Button.Deconstruct))
            {
                otherConnectionRelay.dontConnectToRelays = !otherConnectionRelay.dontConnectToRelays;
                PowerRelay.MarkRelaySystemDirty();
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
