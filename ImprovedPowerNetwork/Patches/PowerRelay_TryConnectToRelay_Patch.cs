using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace ImprovedPowerNetwork.Patches
{
    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.TryConnectToRelay))]
    public static class PowerRelay_TryConnectToRelay_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(PowerRelay __instance, PowerRelay relay, ref bool __result)
        {
            if (__instance is null || relay is null)
            {
                return true;
            }


            if (relay is OtherConnectionRelay)
            {
                __result = false;
                return false;
            }

            if (__instance is OtherConnectionRelay && relay is BasePowerRelay)
            {
                __result = false;
                return false;
            }

            if (__instance is OtherConnectionRelay && relay.gameObject.name.Contains("Transmitter"))
            {
                __result = false;
                return false;
            }

            if (!(__instance is OtherConnectionRelay) && !(__instance is BaseInboundRelay) && __instance.gameObject.name.Contains("Transmitter") && !relay.gameObject.name.Contains("Transmitter"))
            {
                __result = false;
                return false;
            }


            if (relay is BaseInboundRelay)
            {
                __result = false;
                return false;
            }

            if (__instance is BaseInboundRelay && !(relay is BasePowerRelay))
            {
                __result = false;
                return false;
            }

            if (!(__instance is BaseInboundRelay) && relay is BasePowerRelay && __instance.gameObject.name.Contains("Transmitter"))
            {
                __result = false;
                return false;
            }

            if (relay != __instance.outboundRelay && (relay.GetType() == typeof(BasePowerRelay) || relay.GetType() == typeof(PowerRelay)) && relay.inboundPowerSources.Where((x) => x.GetType() == typeof(BaseInboundRelay) || x.GetType() == typeof(OtherConnectionRelay)).Any())
            {
                __result = false;
                return false;
            }

            if (__instance.gameObject.name.Contains("Transmitter") && Physics.Linecast(__instance.GetConnectPoint(), relay.GetConnectPoint(), Voxeland.GetTerrainLayerMask()))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}