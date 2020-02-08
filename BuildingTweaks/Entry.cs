using Harmony;
using QModManager.API.ModLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace BuildingTweaks
{
    [QModCore]
    public class Entry
    {
        [QModPatch]
        public static void Patch()
        {
            try
            {
                HarmonyInstance.Create("MrPurple6411.BuildingTweaks").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
    
    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch(nameof(Constructable.CheckFlags))]
    class Constructable_CheckFlags_Patch
    {
        public static void Postfix(ref bool __result)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                __result = true;
            }
        }
    }
    
    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch(nameof(Builder.ValidateOutdoor))]
    class Builder_ValidateOutdoor_Patch
    {
        public static void Postfix(ref bool __result)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch(nameof(Builder.Update))]
    class Builder_Update_Patch
    {
        public static void Postfix()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Builder.placeMinDistance = 0f;
                Builder.allowedOnConstructables = true;
                Builder.allowedInBase = true;
                Builder.allowedInSub = true;
                Builder.allowedOutside = true;
            }
            Constructable component = Builder.prefab.GetComponent<Constructable>();
            Builder.placeMinDistance = component.placeMinDistance;
            Builder.allowedInSub = component.allowedInSub;
            Builder.allowedInBase = component.allowedInBase;
            Builder.allowedOutside = component.allowedOutside;
            Builder.allowedOnConstructables = component.allowedOnConstructables;
        }
    }

    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch(nameof(Builder.UpdateAllowed))]
    class Builder_UpdateAllowed_Patch
    {
        public static void Postfix(ref bool __result)
        {
            List<string> pieces = new List<string>() {
                "BaseFoundation", "BaseRoom",
                "BaseMoonpool", "BaseCorridorI",
                "BaseCorridorL", "BaseCorridorT",
                "BaseCorridorX"
            };
            bool baseCheck = pieces.Contains(Builder.prefab.name);

            List<GameObject> list = new List<GameObject>();
            Builder.GetObstacles(Builder.placePosition, Builder.placeRotation, Builder.bounds, list);
            if (Input.GetKey(KeyCode.LeftControl) && baseCheck && list.Count == 0)
            {
                __result = true;
            }
            list.Clear();
        }
    }
    
    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch(nameof(Builder.GetSurfaceType))]
    class Builder_GetSurfaceType_Patch
    {
        public static void Postfix(Vector3 hitNormal, ref SurfaceType __result)
        {
            if (Input.GetKey(KeyCode.LeftControl) && __result == SurfaceType.Ceiling)
            {
                __result = SurfaceType.Wall;
            }
        }
    }
}
