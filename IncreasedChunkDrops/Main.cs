using System;
using System.Reflection;
using HarmonyLib;
using IncreasedChunkDrops.Configuration;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;

namespace IncreasedChunkDrops
{
    [QModCore]
    public class Main
    {
        internal static Config config = new Config();

        [QModPatch]
        public static void Load()
        {
            config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());

            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}