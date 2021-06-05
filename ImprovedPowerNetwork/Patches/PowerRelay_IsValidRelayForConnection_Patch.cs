namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;
    using System.Linq;
    using UnityEngine;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.IsValidRelayForConnection))]
    public static class PowerRelay_IsValidRelayForConnection_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance, PowerRelay potentialRelay, ref bool __result)
        {
            if (!__result) return;

            if(__instance is null || potentialRelay is null)
            {
                return;
            }

            var subRoot1 = __instance.gameObject.GetComponentInParent<SubRoot>();
            var subRoot2 = potentialRelay.gameObject.GetComponentInParent<SubRoot>();

            if(__instance is not OtherConnectionRelay && __instance is not BaseInboundRelay && !__instance.gameObject.name.Contains("Transmitter") && subRoot1 is not null && subRoot1 == subRoot2)
            {
                __result = false;
                return;
            }

            if(__instance is BaseInboundRelay && (subRoot1 is null || subRoot1 != subRoot2))
            {
                __result = false;
                return;
            }

            if(potentialRelay is OtherConnectionRelay)
            {
                __result = false;
                return;
            }

            if(__instance is OtherConnectionRelay && potentialRelay is BasePowerRelay)
            {
                __result = false;
                return;
            }

            if(__instance is OtherConnectionRelay && potentialRelay.gameObject.name.Contains("Transmitter"))
            {
                __result = false;
                return;
            }

            if(__instance is not OtherConnectionRelay && __instance is not BaseInboundRelay && __instance.gameObject.name.Contains("Transmitter") && !potentialRelay.gameObject.name.Contains("Transmitter"))
            {
                __result = false;
                return;
            }

            if(potentialRelay is BaseInboundRelay)
            {
                __result = false;
                return;
            }

            if(__instance is BaseInboundRelay && potentialRelay is not BasePowerRelay)
            {
                __result = false;
                return;
            }

            if(potentialRelay is BasePowerRelay && __instance.gameObject.name.Contains("Transmitter") && __instance is not BaseInboundRelay)
            {
                __result = false;
                return;
            }

            if(potentialRelay != __instance.outboundRelay && potentialRelay.GetType() == typeof(PowerRelay) && potentialRelay.inboundPowerSources.Any(x => x is OtherConnectionRelay))
            {
                __result = false;
                return;
            }

            if(__instance is OtherConnectionRelay or BaseInboundRelay)
            {
                return;
            }

            if(Main.Config.LOSBlue && __instance.gameObject.name.Contains("Transmitter") && Physics.Linecast(__instance.GetConnectPoint(), potentialRelay.GetConnectPoint(), Voxeland.GetTerrainLayerMask()))
            {
                __result = false;
                return;
            }

            var position1 = __instance.GetConnectPoint(potentialRelay.GetConnectPoint(__instance.GetConnectPoint(potentialRelay.GetConnectPoint())));
            var position2 = potentialRelay.GetConnectPoint(position1);

            if(Vector3.Distance(position1, position2) > __instance.maxOutboundDistance)
            {
                __result = false;
            }
        }
    }

}