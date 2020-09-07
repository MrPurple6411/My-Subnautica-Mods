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
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}