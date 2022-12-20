namespace BetterACU
{
    using Configuration;
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using System.Reflection;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "BetterACU",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        internal readonly Harmony harmony;
        internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
        public readonly string modFolder;

        #endregion

        internal static Config Config { get; private set; }

        private void Awake()
        {
            Config = OptionsPanelHandler.RegisterModOptions<Config>();
            IngameMenuHandler.RegisterOnSaveEvent(Config.Save);

            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}