using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BuildingTweaks
{
    class Builder_Patches
    {
        [HarmonyPatch(typeof(Builder), "GetSurfaceType")]
        internal class Builder_GetSurfaceType_Patch
        {
            public static void Postfix(ref SurfaceType __result)
            {
                if (Input.GetKey(KeyCode.LeftControl) && __result == SurfaceType.Ceiling)
                {
                    __result = SurfaceType.Wall;
                }
            }
        }

        [HarmonyPatch(typeof(Builder), "UpdateAllowed")]
        internal class Builder_UpdateAllowed_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result, ref GameObject ___prefab, ref List<OrientedBounds> ___bounds, ref Vector3 ___placePosition, ref Quaternion ___placeRotation)
            {
                var pieces = new List<string>()
            {
                "BaseFoundation", "BaseRoom",
                "BaseMoonpool", "BaseCorridorI",
                "BaseCorridorL", "BaseCorridorT",
                "BaseCorridorX"
            };
                bool baseCheck = false;
                foreach (string piece in pieces)
                {
                    if (___prefab.name.Contains(piece))
                    {
                        baseCheck = true;
                    }
                }

                var list = new List<Collider>();
                foreach (OrientedBounds orientedBounds in ___bounds)
                {
                    Builder.GetOverlappedColliders(___placePosition, ___placeRotation, orientedBounds.extents, list);
                    if (list.Count > 0)
                    {
                        break;
                    }
                }
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
                {
                    __result = true;
                }
                else if (baseCheck && list.Count == 0)
                {
                    __result = true;
                }
                list.Clear();
            }

            [HarmonyPrefix]
            public static void Prefix(ref bool ___allowedOnConstructables, ref bool ___allowedInBase, ref bool ___allowedInSub, ref bool ___allowedOutside, ref List<SurfaceType> ___allowedSurfaceTypes)
            {
                ___allowedOnConstructables = true;
                ___allowedInBase = true;
                ___allowedInSub = true;
                ___allowedOutside = true;
                if (___allowedSurfaceTypes.Contains(SurfaceType.Wall) && !___allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
                {
                    ___allowedSurfaceTypes.Add(SurfaceType.Ceiling);
                }
            }
        }

        [HarmonyPatch(typeof(Builder), nameof(Builder.ValidateOutdoor))]
        internal class Builder_ValidateOutdoor_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }
    }
}
