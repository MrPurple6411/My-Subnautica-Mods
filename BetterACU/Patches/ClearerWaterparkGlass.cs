namespace BetterACU.Patches
{
    using System.Collections.Generic;
    using System.Linq;
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch]
    public static class ClearerWaterparkGlass
    {
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int Fresnel = Shader.PropertyToID("_Fresnel");
        private static readonly int Shininess = Shader.PropertyToID("_Shininess");
        private static readonly int SpecInt = Shader.PropertyToID("_SpecInt");

        [HarmonyPatch(typeof(Base), nameof(Base.RebuildGeometry))]
        [HarmonyPostfix]
        public static void Postfix(Base __instance)
        {
            var waterParkGeometries = __instance.gameObject.GetComponentsInChildren<Renderer>()?.ToList() ?? new List<Renderer>();
            foreach (var material in waterParkGeometries.Select(largeRoomWaterPark => largeRoomWaterPark != null
                    ? largeRoomWaterPark.material
                    : null)
                .Where(material => material != null && material.name.StartsWith("Large_Aquarium_Room_generic_glass")))
            {
                material.SetColor(Color1, new Color(0f, 0.609009f, 0.8088291f, 0.15f));
                material.SetFloat(Fresnel, 0.9f);
                material.SetFloat(Shininess, 6f);
                material.SetFloat(SpecInt, 5f);
            }
        }
    }
}