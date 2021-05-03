namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.IsUnderConstruction))]
    public static class PowerRelay_IsUnderConstruction_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance, ref bool __result)
        {
            if(__instance.gameObject.TryGetComponent<PowerControl>(out PowerControl powerControl))
            {
                __result = powerControl.constructable != null && !powerControl.constructable.constructed;
            }
        }
    }
}
