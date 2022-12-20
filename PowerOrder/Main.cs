namespace PowerOrder
{
    using HarmonyLib;
    using Configuration;
    using SMLHelper.V2.Handlers;
    using System.Reflection;

    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static Config config = new();
        internal static uGUI_OptionsPanel optionsPanel;
        internal static ManualLogSource logSource;

        #region[Declarations]

        public const string
            MODNAME = "PowerOrder",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        #endregion

        private Main()
        {
            logSource = Logger;
        }

        private void Awake()
        {
            OptionsPanelHandler.RegisterModOptions(new Options());
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "subnautica.powerorder.mod");
            logSource.Log(LogLevel.Info, "Patching complete.");
        }
    }
}