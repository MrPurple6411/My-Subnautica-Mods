namespace ExtraOptions.Patches;

using System;
using HarmonyLib;

[HarmonyPatch]
public static class SkyApplier_Patch
{
    [HarmonyPatch(typeof(SkyApplier), nameof(SkyApplier.HasMoved))]
    [HarmonyFinalizer]
    public static Exception Finalizer(ref Exception __exception, SkyApplier __instance, ref bool __result)
    {
        if(__exception is NullReferenceException)
        {
            __result = (__instance.applyPosition - __instance.gameObject.transform.position).sqrMagnitude >= 4f;
            __exception = null;
            return null;
        }
        return __exception;
    }
}
