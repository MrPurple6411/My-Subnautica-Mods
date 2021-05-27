namespace PowerOrder
{
    using HarmonyLib;
    using Configuration;
    using QModManager.API.ModLoading;
    using QModManager.Utility;
    using SMLHelper.V2.Handlers;
    using System.Reflection;

    [QModCore]
    public class Main
    {
        internal static Config config = new();
        internal static uGUI_OptionsPanel optionsPanel;

        [QModPatch]
        public static void Load()
        {
            OptionsPanelHandler.RegisterModOptions(new Options());
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "subnautica.powerorder.mod");
            Logger.Log(Logger.Level.Info, "Patching complete.");
        }
    }
}