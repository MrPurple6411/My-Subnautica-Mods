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
        #region[Declarations]
        public const string
            MODNAME = "PowerOrder",
            AUTHOR = "MrPurple6411",
            GUID = "subnautica.powerorder.mod",
            VERSION = "1.0.0.0";
        internal static SMLConfig SMLConfig = new();
        internal static ManualLogSource logSource;
        #endregion

        private void Awake()
        {
            logSource = Logger;
            OptionsPanelHandler.RegisterModOptions(new Options());
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
            logSource.LogInfo("Patching complete.");
        }
    }
}