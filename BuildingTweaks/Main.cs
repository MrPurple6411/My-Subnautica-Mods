namespace BuildingTweaks
{
    using Configuration;
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using System.Reflection;    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "BuildingTweaks",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        public static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        internal static ManualLogSource logSource;
        #endregion

        private void Awake()
        {
            logSource = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}