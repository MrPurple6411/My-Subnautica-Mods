using Harmony;
using System.Reflection;
using UnityEngine;

namespace SaveTheSunbeam
{
    public static class Mod
    {
        public static Sprite redSprite;
        public static Sprite blueSprite;

        public static GameObject ship;

        public static bool disableSaving = false;

        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.SaveTheSunbeam");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void TriggerSunbeamLanding()
        {
            StoryGoalCustomEventHandler.main.countdownActive = false;
            disableSaving = true;
        }
    }
}