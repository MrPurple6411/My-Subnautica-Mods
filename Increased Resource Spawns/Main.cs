namespace Increased_Resource_Spawns
{
    using HarmonyLib;
    using Increased_Resource_Spawns.Configuration;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System.Linq;
    using System.Reflection;

    [QModCore]
    public class Main
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            Config.Blacklist = Config.Blacklist.Distinct().ToList();
            Config.WhiteList = Config.WhiteList.Distinct().ToList();
            Config.Save();


            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}