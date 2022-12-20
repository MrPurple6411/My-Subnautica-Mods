namespace Increased_Resource_Spawns
{
    using HarmonyLib;
    using Configuration;
    using SMLHelper.V2.Handlers;
    using System.Linq;
    using System.Reflection;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        #region[Declarations]

        public const string
            MODNAME = "Increased_Resource_Spawns",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        #endregion

        private void Awake()
        {
            Config.Blacklist = Config.Blacklist.Distinct().ToList();
            Config.WhiteList = Config.WhiteList.Distinct().ToList();
            Config.Save();


            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}