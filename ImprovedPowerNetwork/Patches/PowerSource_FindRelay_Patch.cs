using HarmonyLib;

namespace ImprovedPowerNetwork.Patches
{
    [HarmonyPatch(typeof(PowerSource), nameof(PowerSource.FindRelay))]
    public static class PowerSource_FindRelay_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref PowerRelay __result)
        {
            if(__result != null && (__result is BasePowerRelay || __result.gameObject.name.Contains("Cyclops")))
            {
                PowerControl powerControl = UWE.Utils.GetEntityRoot(__result.gameObject).GetComponentInChildren<PowerControl>();

                if(powerControl != null)
                {
                    if (__result.gameObject.name.Contains("Cyclops"))
                    {
                        __result.AddInboundPower(powerControl.powerRelay);
                    }
                    __result = powerControl.powerRelay;
                }

            }
        }
    }
}
