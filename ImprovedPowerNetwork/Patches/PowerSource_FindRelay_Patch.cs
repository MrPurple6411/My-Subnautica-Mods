namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;
    using System.Linq;

    [HarmonyPatch(typeof(PowerSource), nameof(PowerSource.FindRelay))]
    public static class PowerSource_FindRelay_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref PowerRelay __result)
        {
            PowerControl powerControl;
            bool isCyclops = __result?.gameObject.name.Contains("Cyclops") ?? false;

            if(__result != null && (__result is BasePowerRelay || isCyclops))
            {
                IPowerInterface powerInterface = __result.inboundPowerSources.Where((x) => x is BaseInboundRelay)?.FirstOrFallback(null);

                if(powerInterface is null)
                {
                    powerControl = UWE.Utils.GetEntityRoot(__result.gameObject).GetComponentInChildren<PowerControl>();

                    if(powerControl?.powerRelay != null && !powerControl.powerRelay.dontConnectToRelays)
                    {
                        if(isCyclops)
                        {
                            __result.AddInboundPower(powerControl.powerRelay);
                        }
                        __result = powerControl.powerRelay;
                        return;
                    }
                    return;
                }

                BaseInboundRelay baseInboundRelay = powerInterface as BaseInboundRelay;

                if(baseInboundRelay.gameObject.TryGetComponent(out powerControl))
                {
                    if(powerControl?.powerRelay != null && !powerControl.powerRelay.dontConnectToRelays)
                    {
                        if(isCyclops)
                        {
                            __result.AddInboundPower(powerControl.powerRelay);
                        }
                        __result = powerControl.powerRelay;
                        return;
                    }
                }
            }
        }
    }
}
