namespace NoMagicLights
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UWE;

    [QModCore]
    public class Main
    {
        [QModPostPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}