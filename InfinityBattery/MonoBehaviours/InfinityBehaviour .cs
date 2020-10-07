using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InfinityBattery.MonoBehaviours
{
    internal class InfinityBehaviour : MonoBehaviour
    {
        Battery battery;
        MeshRenderer renderer;
        SkinnedMeshRenderer skinnedRenderer;
        EnergyMixin energyMixin;
        public static Texture illum;

        public float[] strength = new float[] { 1f, 10f};

        public int currentIndex = 0;
        private int nextIndex;

        public float changeColourTime = 2.0f;

        private float timer = 0.0f;

        public void Start()
        {
            battery = gameObject.GetComponent<Battery>();
            renderer = gameObject.GetComponentInChildren<MeshRenderer>();
            skinnedRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

            if(renderer != null && illum == null)
            {
                illum = renderer.material.GetTexture(ShaderPropertyID._Illum);
            }

            if(skinnedRenderer != null && illum != null)
            {
                skinnedRenderer.material.shader = Shader.Find("MarmosetUBER");
                skinnedRenderer.material.EnableKeyword("_EnableGlow");
                skinnedRenderer.material.SetTexture(ShaderPropertyID._Illum, illum);
                skinnedRenderer.material.SetColor("_GlowColor", new Color(1f, 1f, 1f));
            }
            
            
            energyMixin = gameObject.GetComponentInParent<EnergyMixin>();

            nextIndex = (currentIndex + 1) % strength.Length;
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

            if(renderer != null)
            {
                timer += Time.deltaTime;

                if (timer > changeColourTime)
                {
                    currentIndex = (currentIndex + 1) % strength.Length;
                    nextIndex = (currentIndex + 1) % strength.Length;
                    timer = 0.0f;

                }
                renderer.material.SetFloat(ShaderPropertyID._GlowStrength, Mathf.Lerp(strength[currentIndex], strength[nextIndex], timer / changeColourTime));
                renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, Mathf.Lerp(strength[currentIndex], strength[nextIndex], timer / changeColourTime));
            }

            if (skinnedRenderer != null)
            {

                timer += Time.deltaTime;

                if (timer > changeColourTime)
                {
                    currentIndex = (currentIndex + 1) % strength.Length;
                    nextIndex = (currentIndex + 1) % strength.Length;
                    timer = 0.0f;

                }

                skinnedRenderer.material.SetFloat(ShaderPropertyID._GlowStrength, Mathf.Lerp(strength[currentIndex], strength[nextIndex], timer / changeColourTime));
                skinnedRenderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, Mathf.Lerp(strength[currentIndex], strength[nextIndex], timer / changeColourTime));
            }

        }
    }
}
