namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(TechLight), nameof(TechLight.GetNearestValidRelay))]
    public static class TechLight_GetNearestValidRelay_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref PowerRelay __result, GameObject fromObject)
        {
            PowerRelay result = null;
            var num = float.MaxValue;
            foreach (var powerRelay in PowerRelay.relayList)
            {
                if (powerRelay is not OtherConnectionRelay || !powerRelay.gameObject.activeInHierarchy ||
                    !powerRelay.enabled || powerRelay.dontConnectToRelays ||
                    Builder.GetGhostModel() == powerRelay.gameObject)
                {
                    continue;
                }

                var position = fromObject.transform.position;
                var magnitude = (powerRelay.GetConnectPoint(position) - position).magnitude;
                if (!(magnitude <= TechLight.connectionDistance) || !(magnitude < num)) continue;
                num = magnitude;
                result = powerRelay;
            }

            if (result == null) return true;
            __result = result;
            return false;
        }
    }
}
