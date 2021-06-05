namespace ImprovedPowerNetwork
{
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using System.Linq;

    internal class PowerControl: MonoBehaviour, IHandTarget
    {
        private PowerRelay powerRelay;
        private Constructable _constructable;
        private BaseInboundRelay baseInboundRelay;
        private Vehicle vehicle;
        private SubRoot subRoot;
        private bool _vehicleAndEnergyInterfaceNotNull;
        private bool powerFXNotNull;
#if SN1
        private bool _isCyclops;
#elif BZ
        private SeaTruckSegment truckSegment;
#endif

        internal readonly List<OtherConnectionRelay> otherConnectionRelays = new();
        public bool BaseConnectionsDisabled { get; private set; } = true;
        public bool OtherConnectionsDisabled { get; private set; }

        public PowerRelay Relay => powerRelay ??= gameObject.GetComponent<PowerRelay>();
        public Constructable Constructable => _constructable ??= gameObject.GetComponent<Constructable>();

        public void Start()
        {
            powerFXNotNull = Relay.powerFX != null;
            Relay.maxOutboundDistance = Main.Config.BlueBeamRange;

            vehicle = gameObject.GetComponentInParent<Vehicle>();
            subRoot = gameObject.GetComponentInParent<SubRoot>();
            if(subRoot != null)
            {
                if(!subRoot.name.Contains("Cyclops"))
                {
                    BaseConnectionsDisabled = false;
                }
#if SN1
                else
                {
                    _isCyclops = true;
                }
#endif
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
            
            _vehicleAndEnergyInterfaceNotNull = (vehicle?.energyInterface) != null;
        }

        public void LateUpdate()
        {
#if SN1
            if(_isCyclops && GameModeUtils.RequiresPower())
            {
                var chargeNeeded = subRoot.powerRelay.GetMaxPower() - subRoot.powerRelay.GetPower();
                subRoot.powerRelay.AddEnergy(chargeNeeded, out var amountStored);
                Relay.GetEndpoint().ConsumeEnergy(amountStored, out var amountConsumed);

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
                Relay.GetEndpoint().ConsumeEnergy(amountStored, out var amountConsumed);

                if(amountStored > amountConsumed)
                    vehicle.energyInterface.ConsumeEnergy(amountStored - amountConsumed);
            }


            Relay.maxOutboundDistance = Main.Config.BlueBeamRange;

            if(Relay.outboundRelay is not null)
            {
                var position1 = Relay.GetConnectPoint(Relay.outboundRelay.GetConnectPoint(Relay.GetConnectPoint(Relay.outboundRelay.GetConnectPoint())));
                var position2 = Relay.outboundRelay.GetConnectPoint(position1);

                if(Vector3.Distance(position1, position2) > Relay.maxOutboundDistance)
                {
                    Relay.DisconnectFromRelay();
                    return;
                }

                if(Main.Config.LOSBlue && Physics.Linecast(position1, position2, Voxeland.GetTerrainLayerMask()))
                {
                    Relay.DisconnectFromRelay();
                    return;
                }
            }
            else if(Constructable is not null && Constructable.constructed)
            {
                Relay.UpdateConnection();
            }

            var target = powerFXNotNull ? Relay.powerFX.target : null;
            if(target is not null)
                Relay.powerFX.SetTarget(target);

            if(!otherConnectionRelays.Any(otherConnectionRelay => otherConnectionRelay.outboundRelay is null))
                OtherConnectionRelay.AddNewOtherConnectionRelay(Relay, this);

            if(baseInboundRelay is null && !BaseConnectionsDisabled)
                baseInboundRelay = BaseInboundRelay.AddNewBaseConnectionRelay(Relay, this);
        }

        public void OnDisable()
        {
            if (Constructable == null || Constructable.constructed) return;
            if(Relay.outboundRelay != null)
            {
                Relay.outboundRelay.RemoveInboundPower(Relay);
                Relay.outboundRelay = null;
                Relay.powerFX.target = null;
                Destroy(Relay.powerFX.vfxEffectObject);
            }

            PowerRelay.MarkRelaySystemDirty();
        }

        public void OnEnable()
        {
            if(Constructable != null && Constructable.constructed)
            {
                PowerRelay.MarkRelaySystemDirty();
            }
        }

        public void OnHandClick(GUIHand hand)
        {
            if(hand.IsTool())
                return;
            Relay.dontConnectToRelays = !Relay.dontConnectToRelays;
            if(Relay.dontConnectToRelays)
            {
                baseInboundRelay.dontConnectToRelays = Relay.dontConnectToRelays;
                baseInboundRelay.DisconnectFromRelay();
                otherConnectionRelays.ForEach((x) =>
                {
                    x.dontConnectToRelays = Relay.dontConnectToRelays;
                    x.DisconnectFromRelay();
                });
            }
            else
            {
                baseInboundRelay.dontConnectToRelays = BaseConnectionsDisabled;
                baseInboundRelay.DisconnectFromRelay();
                otherConnectionRelays.ForEach((x) =>
                {
                    x.dontConnectToRelays = OtherConnectionsDisabled;
                    x.DisconnectFromRelay();
                });
            }
            Relay.DisconnectFromRelay();
            PowerRelay.MarkRelaySystemDirty();
        }

        public void OnHandHover(GUIHand hand)
        {
            if(hand.IsTool())
                return;
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Max Power {(int)Relay.GetEndpoint().GetMaxPower()}, Current Power: {(int)Relay.GetEndpoint().GetPower()}");
            stringBuilder.Append($"MainConnections: {!Relay.dontConnectToRelays}, ");
            stringBuilder.Append($"Other Connections: {!OtherConnectionsDisabled}");

            var stringBuilder2 = new StringBuilder();

            stringBuilder2.AppendLine("LeftHand: Full Enable/Disable");
            stringBuilder2.AppendLine("Deconstruct Key (Q): Other Connections (Green)");

#if SN1
            HandReticle.main.SetInteractText(stringBuilder.ToString(), stringBuilder2.ToString(), false, false, HandReticle.Hand.None);
#elif BZ
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, stringBuilder.ToString());
                HandReticle.main.SetTextRaw(HandReticle.TextType.HandSubscript, stringBuilder2.ToString());
#endif

            if(!GameInput.GetButtonDown(GameInput.Button.Deconstruct) || Relay.dontConnectToRelays)
                return;
            OtherConnectionsDisabled = !OtherConnectionsDisabled;
            otherConnectionRelays.ForEach((x) =>
            {
                x.dontConnectToRelays = OtherConnectionsDisabled;
                x.DisconnectFromRelay();
            });
            PowerRelay.MarkRelaySystemDirty();
        }

    }
}
