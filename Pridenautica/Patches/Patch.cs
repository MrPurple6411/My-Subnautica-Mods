namespace Pridenautica.Patches
{
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Utility;
    using UnityEngine;
    using HarmonyLib;

    [HarmonyPatch]
    public static class Patch
    {
        private static Texture2D rainbowTexture;

        private static Texture2D RainbowTexture => rainbowTexture ??= ImageUtils.LoadTextureFromFile(
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "Rainbow.png"))?? throw new FileNotFoundException(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "Rainbow.png"));

        [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
        [HarmonyPostfix]
        public static void Postfix(Creature __instance)
        {
            foreach (var renderer in __instance.GetComponentsInChildren<Renderer>())
            {
                foreach (var material in renderer.materials)
                {
                    material.SetTexture(ShaderPropertyID._SpecTex, RainbowTexture);
                }
            }
        }
    }
}
