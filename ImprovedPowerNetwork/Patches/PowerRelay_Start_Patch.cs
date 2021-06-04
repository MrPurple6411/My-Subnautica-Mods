namespace ImprovedPowerNetwork.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.Start))]
    public static class PowerRelay_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PowerRelay __instance)
        {

            if(__instance.transform.position == Vector3.zero && __instance.gameObject.name.Contains("Transmitter"))
            {
                Object.Destroy(__instance.gameObject);
                return;
            }

            if(__instance.gameObject.name.Contains("Transmitter"))
            {
                __instance.gameObject.EnsureComponent<PowerControl>();
            }
        }
    }
}