using System.Reflection;
using HarmonyLib;
using Increased_Resource_Spawns.Configuration;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;

namespace Increased_Resource_Spawns
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