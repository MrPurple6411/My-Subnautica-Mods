namespace TechPistol.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnPilotModeBegin))]
    public static class Vehicle_OnPilotModeBegin_Patches
    {
        [HarmonyPostfix]
        public static void Postfix(Vehicle __instance)
        {
            if(__instance.transform.localScale.x < Vector3.one.x || __instance.transform.localScale.y < Vector3.one.y || __instance.transform.localScale.z < Vector3.one.z)
                Player.main.gameObject.transform.localScale = __instance.transform.localScale;
        }
    }

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnPilotModeEnd))]
    public static class Vehicle_OnPilotModeEnd_Patches
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Player.main.transform.localScale = Vector3.one;
        }
    }
}
