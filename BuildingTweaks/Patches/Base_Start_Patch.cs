namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(Base), nameof(Base.Start))]
    public static class Base_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Base __instance)
        {
            if(__instance.waitingForWorld)
            {
                __instance.RebuildGeometry();
            }

            if(!__instance.TryGetComponent(out Rigidbody rigidbody))
                return;

            Vehicle vehicle = __instance.GetComponentInParent<Vehicle>();

            if(vehicle != null)
            {
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                return;
            }

            Creature creature = __instance.GetComponentInParent<Creature>();

            if(creature != null)
            {
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                return;
            }
#if BZ
            SeaTruckSegment truckSegment = __instance.GetComponentInParent<SeaTruckSegment>();

            if (truckSegment != null)
            {
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                return;
            }
#endif
        }
    }

    [HarmonyPatch(typeof(Base), nameof(Base.Update))]
    public static class Base_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Base __instance)
        {
            if(__instance.waitingForWorld)
            {
                __instance.waitingForWorld = false;
                __instance.RebuildGeometry();
            }
        }
    }
}
