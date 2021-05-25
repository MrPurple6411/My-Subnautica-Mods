namespace BetterACU.Patches
{
    using System.Collections.Generic;
    using System.Linq;
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch]
    public static class ClearerWaterparkGlass
    {
        [HarmonyPatch(typeof(Base), nameof(Base.RebuildGeometry))]
        [HarmonyPostfix]
        public static void Postfix(Base __instance)
        {
            var waterParkGeometries = __instance.gameObject.GetComponentsInChildren<Renderer>()?.ToList() ?? new List<Renderer>();
            foreach(var largeRoomWaterPark in waterParkGeometries)
            {
                var material = largeRoomWaterPark?.material;
                if(material != null && material.name.StartsWith("Large_Aquarium_Room_generic_glass"))
                {
                    material.SetColor("_Color", new Color(0f, 0.609009f, 0.8088291f, 0.15f));
                    material.SetFloat("_Fresnel", 0.9f);
                    material.SetFloat("_Shininess", 6f);
                    material.SetFloat("_SpecInt", 5f);
                }
            }
        }
    }
}