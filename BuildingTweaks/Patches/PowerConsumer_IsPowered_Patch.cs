#if BZ
namespace BuildingTweaks.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(PowerConsumer), nameof(PowerConsumer.IsPowered))]
    public static class PowerConsumer_IsPowered_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(PowerConsumer __instance, ref bool __result)
        {
            if(__instance.baseComp != null || __instance.powerRelay == null)
                return true;

            __result = __instance.powerRelay.IsPowered();
            return false;
        }
    }
}
#endif