namespace ImprovedPowerNetwork
{
    using UnityEngine;

    public class OtherConnectionRelay: PowerRelay
    {
        internal static void AddNewOtherConnectionRelay(PowerRelay originalRelay, PowerControl powerControl)
        {
            var additionalRelay = originalRelay.gameObject.AddComponent<OtherConnectionRelay>();
            additionalRelay.dontConnectToRelays = powerControl.OtherConnectionsDisabled;
            additionalRelay.maxOutboundDistance = Main.Config.GreenBeamRange;
            additionalRelay.constructable = originalRelay.constructable;

            if(originalRelay.powerFX?.vfxPrefab is not null)
            {
                var powerFX = originalRelay.gameObject.AddComponent<OtherRelayPowerFX>();
                powerFX.attachPoint = originalRelay.powerFX.attachPoint;
                powerFX.vfxPrefab = Instantiate(originalRelay.powerFX.vfxPrefab);
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

            if(outboundRelay is not null)
            {
                var position1 = GetConnectPoint(outboundRelay.GetConnectPoint(GetConnectPoint(outboundRelay.GetConnectPoint())));
                var position2 = outboundRelay.GetConnectPoint(position1);

                if(Vector3.Distance(position1, position2) > maxOutboundDistance)
                {
                    DisconnectFromRelay();
                    return;
                }

                var target = powerFX?.target;
                if(target is not null)
                {
                    powerFX.SetTarget(target);
                }
            }
            else if(constructable is not null && constructable.constructed)
            {
                UpdateConnection();
            }
        }

        public void OnEnable()
        {
            if(constructable is not null && constructable.constructed)
            {
                MarkRelaySystemDirty();
            }
        }

        public void OnDisable()
        {
            if (outboundRelay == null) return;
            outboundRelay.RemoveInboundPower(this);
            outboundRelay = null;
            powerFX.target = null;
            Destroy(powerFX.vfxEffectObject);
        }

        public new void OnDestroy()
        {
            if(constructable is not null && !constructable.constructed)
            {
                DisconnectFromRelay();
            }
            base.OnDestroy();
        }

        public class OtherRelayPowerFX: PowerFX { }
    }
}