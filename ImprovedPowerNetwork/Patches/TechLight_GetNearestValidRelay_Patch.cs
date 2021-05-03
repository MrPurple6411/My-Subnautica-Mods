namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(TechLight), nameof(TechLight.GetNearestValidRelay))]
    public static class TechLight_GetNearestValidRelay_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(TechLight __instance, ref PowerRelay __result, GameObject fromObject)
        {

            PowerRelay result = null;
            float num = float.MaxValue;
            for(int i = 0; i < PowerRelay.relayList.Count; i++)
            {
                PowerRelay powerRelay = PowerRelay.relayList[i];
                if(powerRelay is OtherConnectionRelay && powerRelay.gameObject.activeInHierarchy && powerRelay.enabled && !powerRelay.dontConnectToRelays && !(Builder.GetGhostModel() == powerRelay.gameObject))
                {
                    float magnitude = (powerRelay.GetConnectPoint(fromObject.transform.position) - fromObject.transform.position).magnitude;
                    if(magnitude <= TechLight.connectionDistance && magnitude < num)
                    {
                        num = magnitude;
                        result = powerRelay;
                    }
                }
            }

            if(result != null)
            {
                __result = result;
                return false;
            }

            return true;
        }
    }
}
