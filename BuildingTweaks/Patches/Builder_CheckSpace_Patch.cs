namespace BuildingTweaks.Patches
{
    using System;
    using System.Collections.Generic;
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch]
    public static class Builder_ObsticleChecks_Patches
    {
        [HarmonyPatch(typeof(Builder), nameof(Builder.CheckSpace), new Type[] { typeof(Vector3), typeof(Quaternion), typeof(List<OrientedBounds>), typeof(int), typeof(Collider), typeof(List<GameObject>) })]
        [HarmonyPostfix]
        public static void CheckSpace_Postfix(ref List<GameObject> obstacles)
        {
            if(Main.Config.FullOverride || Main.Config.AttachToTarget)
            {
                obstacles.Clear();
            }
        }

        [HarmonyPatch(typeof(Builder), nameof(Builder.GetObstacles))]
        [HarmonyPostfix]
        public static void GetObstacles_Postfix(ref List<GameObject> results)
        {
            if(Main.Config.FullOverride || Main.Config.AttachToTarget)
            {
                results.Clear();
            }
        }
    }
}
