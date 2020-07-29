using System;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Debug = UnityEngine.Debug;

namespace BuilderModule
{
    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            try
            {
                var buildermodule = new BuilderModulePrefab();
                buildermodule.Patch();
                new Harmony("MrPurple6411.BuilderModule").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}