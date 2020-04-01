using Harmony;
using QModManager.API.ModLoading;
using System;
using System.Reflection;

namespace Common
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}