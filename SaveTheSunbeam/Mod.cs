using Harmony;
using System.Reflection;
using UnityEngine;

namespace SaveTheSunbeam
{
    public static class Mod
    {
        public static Sprite redSprite;
        public static Sprite blueSprite;

        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.SaveTheSunbeam");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}