using System;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace Time_Eternal
{
    [QModCore]
    public static class Main
    {
        internal static Config config = new Config();

        [QModPatch]
        public static void Load()
        {
            try
            {
                config.Load();
                OptionsPanelHandler.RegisterModOptions(new Options());

                var assembly = Assembly.GetExecutingAssembly();
                new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}