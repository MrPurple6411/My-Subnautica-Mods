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
            if (__result is null) return;
            var isCyclops = __result.gameObject.name.Contains("Cyclops");
            if(__result is not BasePowerRelay && !isCyclops) return;
            var powerInterface = __result.inboundPowerSources.Where((x) => x is BaseInboundRelay).FirstOrFallback(null);


            PowerControl powerControl;
            if(powerInterface is null)
            {
                var entityRoot = UWE.Utils.GetEntityRoot(__result.gameObject) ?? __result.gameObject;
                powerControl = entityRoot.GetComponentInChildren<PowerControl>();

                if (powerControl?.Relay is null || powerControl.Relay.dontConnectToRelays) return;
                
                if(isCyclops)
                    __result.AddInboundPower(powerControl.Relay);
                
                __result = powerControl.Relay;
                return;
            }

            if (powerInterface is not BaseInboundRelay baseInboundRelay || !baseInboundRelay.gameObject.TryGetComponent(out powerControl)) return;

            if (powerControl.Relay == null || powerControl.Relay.dontConnectToRelays) return;
            
            if(isCyclops)
            {
                __result.AddInboundPower(powerControl.Relay);
            }
            __result = powerControl.Relay;
        }
    }
}
