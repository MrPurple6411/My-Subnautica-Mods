namespace BuildingTweaks
{
    using Configuration;
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using System.Reflection;
    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        public static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        #region[Declarations]

        public const string
            MODNAME = "BuildingTweaks",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        internal readonly Harmony harmony;
        internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
        public readonly string modFolder;
        internal static ManualLogSource logSource;

        #endregion

        private void Awake()
        {
            logSource = Logger;
            var assembly = Assembly.GetExecutingAssembly();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}