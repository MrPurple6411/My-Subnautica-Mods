using HarmonyLib;
using Oculus.Newtonsoft.Json;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using System;
using System.Collections.Generic;
using System.Reflection;
using static LootDistributionData;

namespace JunkDebugging
{
    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

    }

}