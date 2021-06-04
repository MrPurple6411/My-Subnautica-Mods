namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;
    using System.Linq;
    using UnityEngine;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.TryConnectToRelay))]
    public static class PowerRelay_TryConnectToRelay_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(PowerRelay __instance, PowerRelay relay, ref bool __result)
        {
            if(__instance is null || relay is null)
            {
                return true;
            }

            var subRoot1 = __instance.gameObject.GetComponentInParent<SubRoot>();
            var subRoot2 = relay.gameObject.GetComponentInParent<SubRoot>();

            if(__instance is not OtherConnectionRelay && __instance is not BaseInboundRelay && !__instance.gameObject.name.Contains("Transmitter") && subRoot1 != null && subRoot1 == subRoot2)
            {
                __result = false;
                return false;
            }

            if(__instance is BaseInboundRelay && (subRoot1 is null || subRoot1 != subRoot2))
            {
                __result = false;
                return false;
            }

            if(relay is OtherConnectionRelay)
            {
                __result = false;
                return false;
            }

            if(__instance is OtherConnectionRelay && relay is BasePowerRelay)
            {
                __result = false;
                return false;
            }

            if(__instance is OtherConnectionRelay && relay.gameObject.name.Contains("Transmitter"))
            {
                __result = false;
                return false;
            }

            if(__instance is not OtherConnectionRelay && __instance is not BaseInboundRelay && __instance.gameObject.name.Contains("Transmitter") && !relay.gameObject.name.Contains("Transmitter"))
            {
                __result = false;
                return false;
            }


            if(relay is BaseInboundRelay)
            {
                __result = false;
                return false;
            }

            if(__instance is BaseInboundRelay && relay is not BasePowerRelay)
            {
                __result = false;
                return false;
            }

            if(__instance is not BaseInboundRelay && relay is BasePowerRelay && __instance.gameObject.name.Contains("Transmitter"))
            {
                __result = false;
                return false;
            }

            if(relay != __instance.outboundRelay && (relay.GetType() == typeof(BasePowerRelay) || relay.GetType() == typeof(PowerRelay)) && relay.inboundPowerSources.Any(x => x.GetType() == typeof(BaseInboundRelay) || x.GetType() == typeof(OtherConnectionRelay)))
            {
                __result = false;
                return false;
            }

            if(Main.Config.LOSBlue && __instance is not OtherConnectionRelay && __instance is not BaseInboundRelay && __instance.gameObject.name.Contains("Transmitter") && Physics.Linecast(__instance.GetConnectPoint(), relay.GetConnectPoint(), Voxeland.GetTerrainLayerMask()))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}