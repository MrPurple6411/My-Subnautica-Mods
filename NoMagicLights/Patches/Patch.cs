namespace NoMagicLights.Patches
{
    using HarmonyLib;
    using UnityEngine;


    [HarmonyPatch]
    public class Patch
    {
        //[HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.Awake))]
        //[HarmonyPatch(typeof(Drillable), nameof(Drillable.Start))]
        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake))]
        [HarmonyPostfix]
        public static void Postfix(object __instance)
        {
            var component = __instance as Component;
            if (component == null) return;

            var isCreature = component.gameObject.GetComponent<Creature>() != null;

            foreach (var renderer in component.gameObject.GetComponentsInChildren<Renderer>()?? new Renderer[0])
            {
                foreach (var material in renderer.materials)
                {
                    if (!material.HasProperty("_SpecInt")) continue;

                    if (!isCreature)
                    {
                        material.SetFloat("_SpecInt", 0f);
                        continue;
                    }


                    var specInt = material.name.ToLower().Contains("ghost")? 100f :material.GetFloat("_SpecInt");
                    material.SetFloat("_SpecInt", specInt/10f);
                }
            }
        }
    }
}
