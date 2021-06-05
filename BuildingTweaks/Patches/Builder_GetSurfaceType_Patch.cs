using System;

namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using UnityEngine;

    [HarmonyPatch]
    internal class Builder_GetSurfaceType_Patch
    {
        [HarmonyPatch(typeof(Builder), nameof(Builder.GetSurfaceType))]
        [HarmonyPostfix]
        public static void Postfix(ref SurfaceType __result, ref List<SurfaceType> ___allowedSurfaceTypes)
        {
            switch (__result)
            {
                case SurfaceType.Ceiling when ___allowedSurfaceTypes.Contains(SurfaceType.Ceiling):
                    __result = SurfaceType.Ceiling;
                    break;
                case SurfaceType.Ceiling when ___allowedSurfaceTypes.Contains(SurfaceType.Ground):
                    __result = SurfaceType.Ground;
                    break;
                case SurfaceType.Ceiling when ___allowedSurfaceTypes.Contains(SurfaceType.Wall):
                    __result = SurfaceType.Wall;
                    break;
                case SurfaceType.Ground when ___allowedSurfaceTypes.Contains(SurfaceType.Ground):
                    __result = SurfaceType.Ground;
                    break;
                case SurfaceType.Ground when ___allowedSurfaceTypes.Contains(SurfaceType.Ceiling):
                    __result = SurfaceType.Ceiling;
                    break;
                case SurfaceType.Ground when ___allowedSurfaceTypes.Contains(SurfaceType.Wall): 
                    __result = SurfaceType.Wall;
                    break;
                case SurfaceType.Wall:
                    break;
                default:
                    ErrorMessage.AddMessage("Unknown Wall Situation");
                    break;
            }
        }

        [HarmonyPatch(typeof(Builder), nameof(Builder.SetPlaceOnSurface))]
        [HarmonyPrefix]
        public static void Prefix(RaycastHit hit, ref List<SurfaceType> ___allowedSurfaceTypes, ref Vector3 position, ref Quaternion rotation)
        {
            var realSurfaceType = hit.normal.y < -0.33f ? SurfaceType.Ceiling : hit.normal.y < 0.33f ? SurfaceType.Wall : SurfaceType.Ground;
            var localScale = Builder.prefab.transform.localScale;
            var ghostScale = Builder.ghostModelScale;
            switch(realSurfaceType)
            {
                case SurfaceType.Ground:
                    if (___allowedSurfaceTypes.Contains(SurfaceType.Ground) ||
                        ___allowedSurfaceTypes.Contains(SurfaceType.Wall))
                    {
                        if(localScale.y < 0)
                            localScale = new Vector3(localScale.x, localScale.y * -1, localScale.z);
                        if(ghostScale.y < 0)
                            ghostScale = new Vector3(ghostScale.x, ghostScale.y * -1, ghostScale.z);
                    }
                    else if(___allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
                    {
                        if(localScale.y > 0)
                            localScale = new Vector3(localScale.x, localScale.y * -1, localScale.z);
                        if(ghostScale.y > 0)
                            ghostScale = new Vector3(ghostScale.x, ghostScale.y * -1, ghostScale.z);
                        if(!Builder.forceUpright)
                            Builder.forceUpright = true;
                    }
                    break;
                case SurfaceType.Ceiling:
                    if (___allowedSurfaceTypes.Contains(SurfaceType.Ceiling) ||
                        ___allowedSurfaceTypes.Contains(SurfaceType.Wall))
                    {
                        if(localScale.y < 0)
                            localScale = new Vector3(localScale.x, localScale.y * -1, localScale.z);
                        if(ghostScale.y < 0)
                            ghostScale = new Vector3(ghostScale.x, ghostScale.y * -1, ghostScale.z);
                    }
                    else if(___allowedSurfaceTypes.Contains(SurfaceType.Ground))
                    {
                        if(localScale.y > 0)
                            localScale = new Vector3(localScale.x, localScale.y * -1, localScale.z);
                        if(ghostScale.y > 0)
                            ghostScale = new Vector3(ghostScale.x, ghostScale.y * -1, ghostScale.z);
                        if(!Builder.forceUpright)
                            Builder.forceUpright = true;
                    }
                    break;
                case SurfaceType.Wall:
                    break;
                // ReSharper disable once UnreachableSwitchCaseDueToIntegerAnalysis
                default:
                    ErrorMessage.AddMessage("Unknown Wall Situation");
                    break;
            }
            Builder.prefab.transform.localScale = localScale;
            Builder.ghostModelScale = ghostScale;
        }
        
        [HarmonyPatch(typeof(GrowingPlant), nameof(GrowingPlant.SetScale))]
        [HarmonyPostfix]
        public static void Postfix(GrowingPlant __instance, Transform tr, float progress)
        {
            if (Math.Abs(progress - 1f) > 0.01f || tr == __instance.growingTransform ||
                __instance.seed?.currentPlanter?.transform is null || __instance.seed?.currentPlanter?.transform?.localScale.y >= 0 || tr?.localScale.y < 0) return;

            var localScale = tr.localScale;
            localScale = new Vector3(localScale.x, localScale.y*-1, localScale.z);
            tr.localScale = localScale;
            
            if(__instance.passYbounds != null)
                __instance.passYbounds.UpdateWavingScale(tr.localScale);
            else if(__instance.wavingScaler != null)
                __instance.wavingScaler.UpdateWavingScale(tr.localScale);
        }
    }
}
