﻿#if BZ
namespace ExtravagantGifts
{
    using HarmonyLib;using BepInEx;


    public class Main:BaseUnityPlugin
    {
        public void  Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Patches.Patches), $"MrPurple6411_ExtravagantGifts");
        }
    }
}
#endif