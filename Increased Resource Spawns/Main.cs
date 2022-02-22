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
        internal static Config SmcConfig { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void  Awake()
        {
            SmcConfig.Blacklist = SmcConfig.Blacklist.Distinct().ToList();
            SmcConfig.WhiteList = SmcConfig.WhiteList.Distinct().ToList();
            SmcConfig.Save();


            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}