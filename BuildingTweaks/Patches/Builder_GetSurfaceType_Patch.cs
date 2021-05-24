namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [HarmonyPatch]
    internal class Builder_GetSurfaceType_Patch
    {
        [HarmonyPatch(typeof(Builder), nameof(Builder.GetSurfaceType))]
        [HarmonyPostfix]
        public static void Postfix(ref SurfaceType __result, ref List<SurfaceType> ___allowedSurfaceTypes)
        {
            if(__result == SurfaceType.Ceiling)
            {if(___allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
                {
                    __result = SurfaceType.Ceiling;
                }
                else if(___allowedSurfaceTypes.Contains(SurfaceType.Ground))
                {
                    __result = SurfaceType.Ground;
                }
                else if(___allowedSurfaceTypes.Contains(SurfaceType.Wall))
                {
                    __result = SurfaceType.Wall;
                }
            }
            else if (__result == SurfaceType.Ground)
            {
                if(___allowedSurfaceTypes.Contains(SurfaceType.Ground))
                {
                    __result = SurfaceType.Ground;
                }
                else if(___allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
                {
                    __result = SurfaceType.Ceiling;
                }
                else if(___allowedSurfaceTypes.Contains(SurfaceType.Wall))
                {
                    __result = SurfaceType.Wall;
                }
            }
        }

        [HarmonyPatch(typeof(Builder), nameof(Builder.SetPlaceOnSurface))]
        [HarmonyPrefix]
        public static void Prefix(RaycastHit hit, ref List<SurfaceType> ___allowedSurfaceTypes, ref Vector3 position, ref Quaternion rotation)
        {
            SurfaceType realSurfaceType = hit.normal.y < -0.33f ? SurfaceType.Ceiling : hit.normal.y < 0.33f ? SurfaceType.Wall : SurfaceType.Ground;
            switch(realSurfaceType)
            {
                case SurfaceType.Ground:
                    if(___allowedSurfaceTypes.Contains(SurfaceType.Ground) || ___allowedSurfaceTypes.Contains(SurfaceType.Wall))
                    {
                        if(Builder.prefab.transform.localScale.y < 0)
                            Builder.prefab.transform.localScale = new Vector3(Builder.prefab.transform.localScale.x, Builder.prefab.transform.localScale.y * -1, Builder.prefab.transform.localScale.z);
                        if(Builder.ghostModelScale.y < 0)
                            Builder.ghostModelScale = new Vector3(Builder.ghostModelScale.x, Builder.ghostModelScale.y * -1, Builder.ghostModelScale.z);
                    }
                    else if(___allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
                    {
                        if(Builder.prefab.transform.localScale.y > 0)
                            Builder.prefab.transform.localScale = new Vector3(Builder.prefab.transform.localScale.x, Builder.prefab.transform.localScale.y * -1, Builder.prefab.transform.localScale.z);
                        if(Builder.ghostModelScale.y > 0)
                            Builder.ghostModelScale = new Vector3(Builder.ghostModelScale.x, Builder.ghostModelScale.y * -1, Builder.ghostModelScale.z);
                        if(!Builder.forceUpright)
                            Builder.forceUpright = true;
                    }
                    break;
                case SurfaceType.Wall:

                    break;
                case SurfaceType.Ceiling:
                    if(___allowedSurfaceTypes.Contains(SurfaceType.Ceiling) || ___allowedSurfaceTypes.Contains(SurfaceType.Wall))
                    {
                        if(Builder.prefab.transform.localScale.y < 0)
                            Builder.prefab.transform.localScale = new Vector3(Builder.prefab.transform.localScale.x, Builder.prefab.transform.localScale.y * -1, Builder.prefab.transform.localScale.z);
                        if(Builder.ghostModelScale.y < 0)
                            Builder.ghostModelScale = new Vector3(Builder.ghostModelScale.x, Builder.ghostModelScale.y * -1, Builder.ghostModelScale.z);
                    }
                    else if(___allowedSurfaceTypes.Contains(SurfaceType.Ground))
                    {
                        if(Builder.prefab.transform.localScale.y > 0)
                            Builder.prefab.transform.localScale = new Vector3(Builder.prefab.transform.localScale.x, Builder.prefab.transform.localScale.y * -1, Builder.prefab.transform.localScale.z);
                        if(Builder.ghostModelScale.y > 0)
                            Builder.ghostModelScale = new Vector3(Builder.ghostModelScale.x, Builder.ghostModelScale.y * -1, Builder.ghostModelScale.z);
                        if(!Builder.forceUpright)
                            Builder.forceUpright = true;
                    }

                    break;
            }
        }
        
        [HarmonyPatch(typeof(GrowingPlant), nameof(GrowingPlant.SetScale))]
        [HarmonyPostfix]
        public static void Postfix(GrowingPlant __instance, Transform tr, float progress)
        {
            if(progress == 1f && tr != __instance.growingTransform && __instance.seed.currentPlanter.transform.localScale.y < 0)
            {
                tr.localScale = new Vector3(tr.localScale.x, tr.localScale.y*-1, tr.localScale.z);
                if(__instance.passYbounds != null)
                {
                    __instance.passYbounds.UpdateWavingScale(tr.localScale);
                    return;
                }
                if(__instance.wavingScaler != null)
                {
                    __instance.wavingScaler.UpdateWavingScale(tr.localScale);
                }

            }
        }
    }
}
