namespace BuildingTweaks.Patches
{
    using System.Collections.Generic;
    using HarmonyLib;
    using UnityEngine;
#if BZ
    using System;
#endif

    [HarmonyPatch]
    public static class Builder_ObsticleChecks_Patches
    {

#if SN1
        [HarmonyPatch(typeof(Builder), nameof(Builder.CheckSpace), new[] { typeof(Vector3), typeof(Quaternion), typeof(List<OrientedBounds>), typeof(int), typeof(Collider) })]
        [HarmonyPrefix]
        public static bool CheckSpace_Postfix(ref bool __result)
        {
            if(Main.Config.FullOverride || Main.Config.AttachToTarget)
            {
                __result = true;
                return false;
            }
            return true;
        }
#elif BZ
        [HarmonyPatch(typeof(Builder), nameof(Builder.CheckSpace), new Type[] { typeof(Vector3), typeof(Quaternion), typeof(List<OrientedBounds>), typeof(int), typeof(Collider), typeof(List<GameObject>) })]
        [HarmonyPostfix]
        public static void CheckSpace_Postfix(ref List<GameObject> obstacles)
        {
            if(Main.Config.FullOverride || Main.Config.AttachToTarget)
            {
                obstacles.Clear();
            }
        }
#endif

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
