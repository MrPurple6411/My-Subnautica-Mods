using System.Linq;

namespace ImprovedPowerNetwork
{
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    internal class PowerControl: MonoBehaviour, IHandTarget
    {
        public PowerRelay powerRelay;
        public Constructable constructable;
        public BaseInboundRelay baseInboundRelay;
        public List<OtherConnectionRelay> otherConnectionRelays = new();
        public Vehicle vehicle;
        public SubRoot subRoot;
#if BZ
        public SeaTruckSegment truckSegment;
#endif

        public bool baseConnectionsDisabled = true;
        public bool otherConnectionsDisabled;
        private bool _isCyclops;
        private bool _vehicleAndEnergyInterfaceNotNull;
        private bool powerFXNotNull;

        public void OnHandClick(GUIHand hand)
        {
            if (hand.IsTool()) return;
            powerRelay.dontConnectToRelays = !powerRelay.dontConnectToRelays;
            if(powerRelay.dontConnectToRelays)
            {
                baseInboundRelay.dontConnectToRelays = powerRelay.dontConnectToRelays;
                baseInboundRelay.DisconnectFromRelay();
                otherConnectionRelays.ForEach((x) =>
                {
                    x.dontConnectToRelays = powerRelay.dontConnectToRelays;
                    x.DisconnectFromRelay();
                });
            }
            else
            {
                baseInboundRelay.dontConnectToRelays = baseConnectionsDisabled;
                baseInboundRelay.DisconnectFromRelay();
                otherConnectionRelays.ForEach((x) =>
                {
                    x.dontConnectToRelays = otherConnectionsDisabled;
                    x.DisconnectFromRelay();
                });
            }
            powerRelay.DisconnectFromRelay();
            PowerRelay.MarkRelaySystemDirty();
        }

        public void OnHandHover(GUIHand hand)
        {
            if (hand.IsTool()) return;
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Max Power {(int)powerRelay.GetEndpoint().GetMaxPower()}, Current Power: {(int)powerRelay.GetEndpoint().GetPower()}");
            stringBuilder.Append($"MainConnections: {!powerRelay.dontConnectToRelays}, ");
            stringBuilder.Append($"Other Connections: {!otherConnectionsDisabled}");

            var stringBuilder2 = new StringBuilder();

            stringBuilder2.AppendLine("LeftHand: Full Enable/Disable");
            stringBuilder2.AppendLine("Deconstruct Key (Q): Other Connections (Green)");

#if SN1
            HandReticle.main.SetInteractText(stringBuilder.ToString(), stringBuilder2.ToString(), false, false, HandReticle.Hand.None);
#elif BZ
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, stringBuilder.ToString());
                HandReticle.main.SetTextRaw(HandReticle.TextType.HandSubscript, stringBuilder2.ToString());
#endif

            if (!GameInput.GetButtonDown(GameInput.Button.Deconstruct) || powerRelay.dontConnectToRelays) return;
            otherConnectionsDisabled = !otherConnectionsDisabled;
            otherConnectionRelays.ForEach((x) =>
            {
                x.dontConnectToRelays = otherConnectionsDisabled;
                x.DisconnectFromRelay();
            });
            PowerRelay.MarkRelaySystemDirty();
        }

        public void Start()
        {
            powerFXNotNull = powerRelay.powerFX != null;
            powerRelay = gameObject.GetComponent<PowerRelay>();
            powerRelay.maxOutboundDistance = Main.Config.BlueBeamRange;
            constructable = gameObject.GetComponent<Constructable>();

            vehicle = gameObject.GetComponentInParent<Vehicle>();
            subRoot = gameObject.GetComponentInParent<SubRoot>();
            if(subRoot != null)
            {
                if(!subRoot.name.Contains("Cyclops"))
                {
                    baseConnectionsDisabled = false;
                }
                else
                {
                    _isCyclops = true;
                }
                    
            }
#if BZ
            truckSegment = gameObject.GetComponentInParent<SeaTruckSegment>();
#endif


            if((vehicle != null || subRoot != null
#if BZ
                || truckSegment != null
#endif
                ) && gameObject.TryGetComponent(out Rigidbody rigidbody))
            {
                Destroy(rigidbody);
            }
            
            _vehicleAndEnergyInterfaceNotNull = (vehicle != null ? vehicle.energyInterface : null) != null;
        }

        public void LateUpdate()
        {
#if SN1
            if(_isCyclops && GameModeUtils.RequiresPower())
            {
                var chargeNeeded = subRoot.powerRelay.GetMaxPower() - subRoot.powerRelay.GetPower();
                subRoot.powerRelay.AddEnergy(chargeNeeded, out var amountStored);
                powerRelay.GetEndpoint().ConsumeEnergy(amountStored, out var amountConsumed);

                if(amountStored > amountConsumed)
                    subRoot.powerRelay.ConsumeEnergy(amountStored - amountConsumed, out _);
            }

#elif BZ
            if(truckSegment?.relay != null && GameModeUtils.RequiresPower())
            {
                var chargeNeeded = truckSegment.relay.GetMaxPower() - truckSegment.relay.GetPower();
                truckSegment.relay.AddEnergy(chargeNeeded, out var amountStored);
                powerRelay.GetEndpoint().ConsumeEnergy(amountStored, out var amountConsumed);

                if (amountStored > amountConsumed)
                    truckSegment.relay.ConsumeEnergy(amountStored - amountConsumed, out _);
            }
#endif
            if(_vehicleAndEnergyInterfaceNotNull && GameModeUtils.RequiresPower())
            {
                vehicle.energyInterface.GetValues(out var charge, out var capacity);
                var chargeNeeded = capacity - charge;
                var amountStored = vehicle.energyInterface.AddEnergy(chargeNeeded);
                powerRelay.GetEndpoint().ConsumeEnergy(amountStored, out var amountConsumed);

                if(amountStored > amountConsumed)
                    vehicle.energyInterface.ConsumeEnergy(amountStored - amountConsumed);
            }


            powerRelay.maxOutboundDistance = Main.Config.BlueBeamRange;

            if(powerRelay.outboundRelay is not null)
            {
                var position1 = powerRelay.GetConnectPoint(powerRelay.outboundRelay.GetConnectPoint(powerRelay.GetConnectPoint(powerRelay.outboundRelay.GetConnectPoint())));
                var position2 = powerRelay.outboundRelay.GetConnectPoint(position1);

                if(Vector3.Distance(position1, position2) > powerRelay.maxOutboundDistance)
                {
                    powerRelay.DisconnectFromRelay();
                    return;
                }

                if(Main.Config.LOSBlue && Physics.Linecast(position1, position2, Voxeland.GetTerrainLayerMask()))
                {
                    powerRelay.DisconnectFromRelay();
                    return;
                }
            }
            else if(constructable is not null && constructable.constructed)
            {
                powerRelay.UpdateConnection();
            }

            var target = powerFXNotNull ? powerRelay.powerFX.target : null;
            if(target is not null)
                powerRelay.powerFX.SetTarget(target);

            if(!otherConnectionRelays.Any(otherConnectionRelay => otherConnectionRelay.outboundRelay is null))
                OtherConnectionRelay.AddNewOtherConnectionRelay(powerRelay, this);

            if(baseInboundRelay is null && !baseConnectionsDisabled)
                baseInboundRelay = BaseInboundRelay.AddNewBaseConnectionRelay(powerRelay, this);
        }

        public void OnDisable()
        {
            if (constructable == null || constructable.constructed) return;
            if(powerRelay.outboundRelay != null)
            {
                powerRelay.outboundRelay.RemoveInboundPower(powerRelay);
                powerRelay.outboundRelay = null;
                powerRelay.powerFX.target = null;
                Destroy(powerRelay.powerFX.vfxEffectObject);
            }

            PowerRelay.MarkRelaySystemDirty();
        }

        public void OnEnable()
        {
            if(constructable != null && constructable.constructed)
            {
                PowerRelay.MarkRelaySystemDirty();
            }
        }

    }
}
