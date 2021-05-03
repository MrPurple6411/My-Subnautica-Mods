namespace TechPistol.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnPlayerEntered))]
    public static class SubRoot_OnPlayerEntered_Patches
    {
        [HarmonyPostfix]
        public static void Postfix(SubRoot __instance)
        {
            GameObject rootObject = UWE.Utils.GetEntityRoot(__instance.gameObject);
            if(rootObject.transform.localScale.x < Vector3.one.x || rootObject.transform.localScale.y < Vector3.one.y || rootObject.transform.localScale.z < Vector3.one.z)
                Player.main.gameObject.transform.localScale = rootObject.transform.localScale;
        }
    }

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnPlayerExited))]
    public static class SubRoot_OnPlayerExited_Patches
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Player.main.transform.localScale = Vector3.one;
        }
    }
}
