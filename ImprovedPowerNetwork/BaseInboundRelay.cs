namespace ImprovedPowerNetwork
{
    using UnityEngine;

    public class BaseInboundRelay: PowerRelay
    {
        internal static BaseInboundRelay AddNewBaseConnectionRelay(PowerRelay originalRelay, PowerControl powerControl)
        {
            BaseInboundRelay additionalRelay = originalRelay.gameObject.AddComponent<BaseInboundRelay>();
            additionalRelay.dontConnectToRelays = powerControl.baseConnectionsDisabled;
            additionalRelay.maxOutboundDistance = 10000;
            additionalRelay.constructable = originalRelay.constructable;

            if(originalRelay.powerFX != null && originalRelay.powerFX.vfxPrefab != null)
            {
                BaseInboundRelayPowerFX powerFX = originalRelay.gameObject.AddComponent<BaseInboundRelayPowerFX>();
                powerFX.attachPoint = originalRelay.powerFX.attachPoint;
                powerFX.vfxPrefab = GameObject.Instantiate(originalRelay.powerFX.vfxPrefab);
                powerFX.vfxPrefab.SetActive(false);
                powerFX.vfxPrefab.GetComponent<LineRenderer>().material.SetColor(ShaderPropertyID._Color, Color.magenta);


                additionalRelay.powerFX = powerFX;
            }
            additionalRelay.AddInboundPower(originalRelay);

            return additionalRelay;
        }

        public void LateUpdate()
        {
            if(outboundRelay is null && (constructable?.constructed ?? false))
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

        public class BaseInboundRelayPowerFX: PowerFX { }
    }
}