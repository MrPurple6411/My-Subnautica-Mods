using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Builder), nameof(Builder.GetSurfaceType))]
    internal class Builder_GetSurfaceType_Patch
    {
        public static void Postfix(ref SurfaceType __result, ref List<SurfaceType> ___allowedSurfaceTypes)
        {
            if (__result == SurfaceType.Ceiling)
            {
                if (___allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
                    __result = SurfaceType.Ceiling;
                else if (___allowedSurfaceTypes.Contains(SurfaceType.Ground))
                    __result = SurfaceType.Ground;
                else if (___allowedSurfaceTypes.Contains(SurfaceType.Wall))
                    __result = SurfaceType.Wall;
            }
        }
    }
}
