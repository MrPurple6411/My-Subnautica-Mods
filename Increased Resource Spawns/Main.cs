using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;

namespace Increased_Resource_Spawns
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