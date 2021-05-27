namespace IslandCloudRemoval.Patches
{
    using HarmonyLib;
    using System.Collections;
    using UnityEngine;
    using UWE;

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            CoroutineHost.StartCoroutine(NewMethod());
        }

        private static IEnumerator NewMethod()
        {
            var x_Clouds = GameObject.Find("x_Clouds(Clone)");

            yield return new WaitUntil(() => (x_Clouds ??= GameObject.Find("x_Clouds(Clone)")) != null);

            var renderers = x_Clouds.GetComponentsInChildren<MeshRenderer>();

            foreach(var renderer in renderers)
            {
                if(renderer.name.Contains("x_IslandClouds"))
                    renderer.gameObject.SetActive(false);
            }
        }
    }
}
