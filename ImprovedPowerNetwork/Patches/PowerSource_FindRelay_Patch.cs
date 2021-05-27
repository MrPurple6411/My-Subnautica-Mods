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
            if (__result == null) return;
            var isCyclops = __result.gameObject.name.Contains("Cyclops");
            if(__result is not BasePowerRelay && !isCyclops) return;
            var powerInterface = __result.inboundPowerSources.Where((x) => x is BaseInboundRelay).FirstOrFallback(null);


            PowerControl powerControl;
            if(powerInterface is null)
            {
                powerControl = UWE.Utils.GetEntityRoot(__result.gameObject).GetComponentInChildren<PowerControl>();

                if (powerControl.powerRelay == null || powerControl.powerRelay.dontConnectToRelays) return;
                
                if(isCyclops)
                    __result.AddInboundPower(powerControl.powerRelay);
                
                __result = powerControl.powerRelay;
                return;
            }

            if (powerInterface is not BaseInboundRelay baseInboundRelay ||
                !baseInboundRelay.gameObject.TryGetComponent(out powerControl)) return;

            if (powerControl.powerRelay == null || powerControl.powerRelay.dontConnectToRelays) return;
            
            if(isCyclops)
            {
                __result.AddInboundPower(powerControl.powerRelay);
            }
            __result = powerControl.powerRelay;
        }
    }
}
