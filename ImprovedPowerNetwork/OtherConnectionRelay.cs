namespace ImprovedPowerNetwork
{
    using UnityEngine;

    public class OtherConnectionRelay: PowerRelay
    {
        internal static void AddNewOtherConnectionRelay(PowerRelay originalRelay, PowerControl powerControl)
        {
            OtherConnectionRelay additionalRelay = originalRelay.gameObject.AddComponent<OtherConnectionRelay>();
            additionalRelay.dontConnectToRelays = powerControl.otherConnectionsDisabled;
            additionalRelay.maxOutboundDistance = Main.Config.GreenBeamRange;
            additionalRelay.constructable = originalRelay.constructable;

            if(originalRelay.powerFX != null && originalRelay.powerFX.vfxPrefab != null)
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

        public void LateUpdate()
        {
            maxOutboundDistance = Main.Config.GreenBeamRange;

            if(outboundRelay != null)
            {
                Vector3 position1 = GetConnectPoint(outboundRelay.GetConnectPoint(GetConnectPoint(outboundRelay.GetConnectPoint())));
                Vector3 position2 = outboundRelay.GetConnectPoint(position1);

                if(Vector3.Distance(position1, position2) > maxOutboundDistance)
                {
                    DisconnectFromRelay();
                    return;
                }

                GameObject target = powerFX?.target;
                if(target != null)
                {
                    powerFX?.SetTarget(target);
                }
            }
            else if(constructable?.constructed ?? false)
            {
                UpdateConnection();
            }
        }

        public void OnEnable()
        {
            if(constructable?.constructed ?? false)
            {
                PowerRelay.MarkRelaySystemDirty();
            }
        }

        public void OnDisable()
        {
            if(outboundRelay != null)
            {
                outboundRelay.RemoveInboundPower(this);
                outboundRelay = null;
                powerFX.target = null;
                GameObject.Destroy(powerFX.vfxEffectObject);
            }
        }

        public new void OnDestroy()
        {
            if(!constructable?.constructed ?? false)
            {
                DisconnectFromRelay();
            }
            base.OnDestroy();
        }

        public class OtherRelayPowerFX: PowerFX { }
    }
}