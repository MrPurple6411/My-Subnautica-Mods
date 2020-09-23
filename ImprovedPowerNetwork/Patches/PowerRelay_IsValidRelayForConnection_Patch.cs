using HarmonyLib;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ImprovedPowerNetwork.Patches
{
    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.IsValidRelayForConnection))]
    public static class PowerRelay_IsValidRelayForConnection_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance, PowerRelay potentialRelay, ref bool __result)
        {
            if (__result)
            {
                if (__instance is null || potentialRelay is null || __instance.outboundRelay == potentialRelay)
                {
                    return;
                }

                if (potentialRelay is OtherConnectionRelay)
                {
                    __result = false;
                    return;
                }

                if (__instance is OtherConnectionRelay && potentialRelay is BasePowerRelay)
                {
                    __result = false;
                    return;
                }

                if (__instance is OtherConnectionRelay && potentialRelay.gameObject.name.Contains("Transmitter"))
                {
                    __result = false;
                    return;
                }

                if (!(__instance is OtherConnectionRelay) && !(__instance is BaseConnectionRelay) && __instance.gameObject.name.Contains("Transmitter") && !potentialRelay.gameObject.name.Contains("Transmitter"))
                {
                    __result = false;
                    return;
                }

                if (potentialRelay is BaseConnectionRelay)
                {
                    __result = false;
                    return;
                }

                if (__instance is BaseConnectionRelay && !(potentialRelay is BasePowerRelay))
                {
                    __result = false;
                    return;
                }

                if (potentialRelay is BasePowerRelay && __instance.gameObject.name.Contains("Transmitter") && !(__instance is BaseConnectionRelay))
                {
                    __result = false;
                    return;
                }

                if ((potentialRelay.GetType() == typeof(BasePowerRelay) || potentialRelay.GetType() == typeof(PowerRelay)) && potentialRelay.inboundPowerSources.Where((x) => x.GetType() == typeof(BaseConnectionRelay) || x.GetType() == typeof(OtherConnectionRelay)).Any())
                {
                    __result = false;
                    return;
                }

                if (__instance.gameObject.name.Contains("Transmitter") && potentialRelay.gameObject.name.Contains("Transmitter") && Physics.Linecast(__instance.GetConnectPoint(), potentialRelay.GetConnectPoint(), Voxeland.GetTerrainLayerMask()))
                {
                    __result = false;
                    return;
                }
            }
        }
    }

}