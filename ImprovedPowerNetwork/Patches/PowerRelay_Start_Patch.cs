using HarmonyLib;

namespace ImprovedPowerNetwork.Patches
{
    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.Start))]
    public static class PowerRelay_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance)
        {
            if (__instance.gameObject.name.Contains("Transmitter"))
            {
                BaseConnectionRelay.EnsureBaseConnectionRelay(__instance);
                OtherConnectionRelay.EnsureOtherConnectionRelay(__instance);
                __instance.gameObject.EnsureComponent<PowerControl>();
            }
        }
    }
}