using BepInEx;

namespace Increased_Resource_Spawns
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Handlers;
    using System.Linq;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void  Awake()
        {
            Config.Blacklist = Config.Blacklist.Distinct().ToList();
            Config.WhiteList = Config.WhiteList.Distinct().ToList();
            Config.Save();


            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}