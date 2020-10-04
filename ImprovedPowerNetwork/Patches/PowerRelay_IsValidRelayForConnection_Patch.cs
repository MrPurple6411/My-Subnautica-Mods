using FMOD;
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
                if (__instance is null || potentialRelay is null)
                {
                    return;
                }

                SubRoot subRoot1 = __instance.gameObject.GetComponentInParent<SubRoot>();
                SubRoot subRoot2 = potentialRelay.gameObject.GetComponentInParent<SubRoot>();

                if (!(__instance is OtherConnectionRelay) && !(__instance is BaseInboundRelay) && subRoot1 != null && subRoot1 == subRoot2)
                {
                    __result = false;
                    return;
                }

                if(subRoot1 != null && subRoot1 != subRoot2 && !__instance.gameObject.name.Contains("Transmitter"))
                {
                    __result = false;
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

                if (!(__instance is OtherConnectionRelay) && !(__instance is BaseInboundRelay) && __instance.gameObject.name.Contains("Transmitter") && !potentialRelay.gameObject.name.Contains("Transmitter"))
                {
                    __result = false;
                    return;
                }

                if (potentialRelay is BaseInboundRelay)
                {
                    __result = false;
                    return;
                }

                if (__instance is BaseInboundRelay && !(potentialRelay is BasePowerRelay))
                {
                    __result = false;
                    return;
                }

                if (potentialRelay is BasePowerRelay && __instance.gameObject.name.Contains("Transmitter") && !(__instance is BaseInboundRelay))
                {
                    __result = false;
                    return;
                }

                if (potentialRelay != __instance.outboundRelay && (potentialRelay.GetType() == typeof(BasePowerRelay) || potentialRelay.GetType() == typeof(PowerRelay)) && potentialRelay.inboundPowerSources.Where((x) => x.GetType() == typeof(BaseInboundRelay) || x.GetType() == typeof(OtherConnectionRelay)).Any())
                {
                    __result = false;
                    return;
                }

                if(__instance is OtherConnectionRelay || __instance is BaseInboundRelay)
                {
                    return;
                }

                if (__instance.gameObject.name.Contains("Transmitter") && Physics.Linecast(__instance.GetConnectPoint(), potentialRelay.GetConnectPoint(), Voxeland.GetTerrainLayerMask()))
                {
                    __result = false;
                    return;
                }

                if (Vector3.Distance(__instance.GetConnectPoint(potentialRelay.GetConnectPoint(__instance.GetConnectPoint(potentialRelay.GetConnectPoint()))), potentialRelay.GetConnectPoint(__instance.GetConnectPoint(potentialRelay.GetConnectPoint()))) > __instance.maxOutboundDistance)
                {
                    __result = false;
                    return;
                }
            }
        }
    }

}