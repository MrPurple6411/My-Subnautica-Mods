namespace UnobtaniumBatteries.MonoBehaviours
{
    using UnityEngine;

    internal class UnobtaniumBehaviour: MonoBehaviour
    {
        private Battery battery;
        private Renderer renderer;
        private EnergyMixin energyMixin;

        private float currentStrength = 0;
        private float nextStrength = 2;
        private float changeTime = 2f;
        private float timer = 0.0f;

        public void Awake()
        {
            renderer = gameObject.GetComponentInChildren<Renderer>();
            battery = gameObject.GetComponent<Battery>();
            energyMixin = gameObject.GetComponentInParent<EnergyMixin>();
        }

        public void Update()
        {
            if(battery != null)
            {
                battery.charge = battery.capacity;
            }

            if(energyMixin != null)
            {
                energyMixin.AddEnergy(energyMixin.capacity - energyMixin.charge);
            }

            timer += Time.deltaTime;

            if(timer > changeTime)
            {
                currentStrength = nextStrength;
                nextStrength = currentStrength == 2 ? 0 : 2;

                timer = 0.0f;
            }

            if(renderer != null)
            {
                renderer.material.SetFloat(ShaderPropertyID._GlowStrength, Mathf.Lerp(currentStrength, nextStrength, timer / changeTime));
                renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, Mathf.Lerp(currentStrength, nextStrength, timer / changeTime));
            }
        }
        public void OnDestroy()
        {
            renderer.material.SetFloat(ShaderPropertyID._GlowStrength, 1f);
            renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, 1f);
        }
    }
}
