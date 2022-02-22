using BepInEx;

namespace NoCrosshair
{
    using HarmonyLib;

    public class Main:BaseUnityPlugin
    {
        public void  Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Patches.Patches), $"MrPurple6411_NoCrosshair");
        }
    }
}