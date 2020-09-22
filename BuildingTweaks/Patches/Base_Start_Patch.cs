using HarmonyLib;
using UnityEngine;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Base), "Start")]
    public static class Base_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Base __instance)
        {
            if(!__instance.isReady)
            {
                __instance.RebuildGeometry();
            }
        }
    }

    [HarmonyPatch(typeof(Base), "Update")]
    public static class Base_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Base __instance)
        {
            if (!__instance.isReady)
            {
                Traverse.Create(__instance).Field<bool>("waitingForWorld").Value = false;
                __instance.RebuildGeometry();
            }
        }
    }
}
