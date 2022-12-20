namespace ImprovedPowerNetwork
{
    using HarmonyLib;
    using Configuration;
    using SMLHelper.V2.Handlers;
    using System.Reflection;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        #region[Declarations]

        public const string
            MODNAME = "ImprovedPowerNetwork",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        #endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}