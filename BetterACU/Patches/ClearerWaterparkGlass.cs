namespace BetterACU.Patches;

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch]
public static class ClearerWaterparkGlass
{
    private static readonly int _color1 = Shader.PropertyToID("_Color");
    private static readonly int _fresnel = Shader.PropertyToID("_Fresnel");
    private static readonly int _shininess = Shader.PropertyToID("_Shininess");
    private static readonly int _specInt = Shader.PropertyToID("_SpecInt");

    [HarmonyPatch(typeof(Base), nameof(Base.RebuildGeometry))]
    [HarmonyPostfix]
    public static void Postfix(Base __instance)
    {
        var waterParkGeometries = __instance.gameObject.GetComponentsInChildren<Renderer>()?.ToList() ?? new List<Renderer>();
        foreach (var material in waterParkGeometries.Select(largeRoomWaterPark => largeRoomWaterPark?.material)
            .Where(material => material != null && material.name.StartsWith("Large_Aquarium_Room_generic_glass")))
        {
            material.SetColor(_color1, new Color(0f, 0.609009f, 0.8088291f, 0.15f));
            material.SetFloat(_fresnel, 0.9f);
            material.SetFloat(_shininess, 6f);
            material.SetFloat(_specInt, 5f);
        }
    }
}