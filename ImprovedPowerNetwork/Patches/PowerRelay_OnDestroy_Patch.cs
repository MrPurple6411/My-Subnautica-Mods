using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedPowerNetwork.Patches
{
    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.OnDestroy))]
    public static class PowerRelay_OnDestroy_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(PowerRelay __instance)
        {
            __instance.gameObject.GetComponent<BaseConnectionRelay>()?.DisconnectFromRelay();
            __instance.gameObject.GetComponent<OtherConnectionRelay>()?.DisconnectFromRelay();
        }
    }
}
