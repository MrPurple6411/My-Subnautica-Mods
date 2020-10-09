using HarmonyLib;
using UnityEngine;

namespace BuildingTweaks.Patches
{
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
        }
    }

    [HarmonyPatch(typeof(Base), nameof(Base.Update))]
    public static class Base_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Base __instance)
        {
            if (__instance.waitingForWorld)
            {
                __instance.waitingForWorld = false;
                __instance.RebuildGeometry();
            }
        }
    }
}
