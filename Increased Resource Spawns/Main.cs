namespace Increased_Resource_Spawns
{
    using HarmonyLib;
    using Configuration;
    using SMLHelper.Handlers;
    using System.Linq;
    using System.Reflection;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "Increased_Resource_Spawns",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        #endregion

        private void Awake()
        {
            SMLConfig.Blacklist = SMLConfig.Blacklist.Distinct().ToList();
            SMLConfig.WhiteList = SMLConfig.WhiteList.Distinct().ToList();
            SMLConfig.Save();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}