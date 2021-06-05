namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(Constructable), nameof(Constructable.Start))]
    public static class Constructable_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Constructable __instance)
        {
            if(!__instance.TryGetComponent(out Rigidbody rigidbody))
                return;

            var vehicle = __instance.GetComponentInParent<Vehicle>();

            if(vehicle != null)
            {
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                return;
            }

            var creature = __instance.GetComponentInParent<Creature>();

            if(creature != null)
            {
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
#if BZ
            var truckSegment = __instance.GetComponentInParent<SeaTruckSegment>();

            if (truckSegment == null) return;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
#endif
        }
    }
}
