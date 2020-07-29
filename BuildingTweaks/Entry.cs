using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BuildingTweaks
{
    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch(nameof(Builder.GetSurfaceType))]
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

    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch(nameof(Builder.UpdateAllowed))]
    internal class Builder_UpdateAllowed_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
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
                if (Builder.prefab.name.Contains(piece))
                {
                    baseCheck = true;
                }
            }

            var list = new List<Collider>();
            foreach (OrientedBounds orientedBounds in Builder.bounds)
            {
                Builder.GetOverlappedColliders(Builder.placePosition, Builder.placeRotation, orientedBounds.extents, list);
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
        public static void Prefix()
        {
            Builder.allowedOnConstructables = true;
            Builder.allowedInBase = true;
            Builder.allowedInSub = true;
            Builder.allowedOutside = true;
            if (Builder.allowedSurfaceTypes.Contains(SurfaceType.Wall) && !Builder.allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
            {
                Builder.allowedSurfaceTypes.Add(SurfaceType.Ceiling);
            }
        }
    }

    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch(nameof(Builder.ValidateOutdoor))]
    internal class Builder_ValidateOutdoor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
}