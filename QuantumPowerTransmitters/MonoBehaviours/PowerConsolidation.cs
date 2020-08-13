using HarmonyLib;
using Oculus.Newtonsoft.Json;
using QModManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace QuantumPowerTransmitters.MonoBehaviours
{
    internal class PowerConsolidation : MonoBehaviour, IPowerInterface
    {
        private List<BasePowerRelay> baseRelays => Main.baseRelays;
        private BasePowerRelay baseRelay;
        private Constructable constructable;
        private BasePowerRelay[] otherRelays => baseRelays.Where((x)=> x.gameObject.GetInstanceID() != baseRelay.gameObject.GetInstanceID()).ToArray();
        private List<IPowerInterface> PowerInterfaces = new List<IPowerInterface>();
        private float nextcheck = 0;

        private void Start()
        {
            baseRelay = this.gameObject.GetComponentInParent<BasePowerRelay>();
            constructable = this.gameObject.GetComponent<Constructable>();
            if(baseRelay != null && !baseRelays.Contains(baseRelay))
            {
                baseRelays.Add(baseRelay);
            }
        }

        private void FixedUpdate()
        {
            if(constructable == null)
            {
                constructable = this.gameObject.GetComponent<Constructable>();
            }

            if (constructable.constructed)
            {
                if(baseRelay == null)
                {
                    baseRelay = this.gameObject.GetComponentInParent<BasePowerRelay>();
                }

                if (!baseRelays.Contains(baseRelay))
                {
                    baseRelays.Add(baseRelay);
                }

                float time = Time.time;
                if (time > nextcheck)
                {
                    Logger.Log(Logger.Level.Info, $"Relays: " + JsonConvert.SerializeObject(baseRelays, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), showOnScreen: true);
                    /*foreach (BasePowerRelay basePower in otherRelays)
                    {
                        List<IPowerInterface> interfaces = (List<IPowerInterface>)AccessTools.Field(typeof(BasePowerRelay), "inboundPowerSources").GetValue(basePower);
                        PowerInterfaces.Concat(interfaces);
                    }*/
                    nextcheck += 15;
                }
            }
        }

        private void OnDestroy()
        {
            if (baseRelays.Contains(baseRelay))
            {
                baseRelays.Remove(baseRelay);
            }
        }

        public bool GetInboundHasSource(IPowerInterface powerInterface)
        {
            return PowerInterfaces.Contains(powerInterface);
        }

        public float GetMaxPower()
        {
            return PowerInterfaces.Sum((x) => x.GetMaxPower());
        }

        public float GetPower()
        {
            return PowerInterfaces.Sum((x) => x.GetPower());
        }

        public bool HasInboundPower(IPowerInterface powerInterface)
        {
            return PowerInterfaces.Contains(powerInterface);
        }

        public bool ModifyPower(float amount, out float modified)
        {
            modified = 0;
            return false;
        }
    }
}
