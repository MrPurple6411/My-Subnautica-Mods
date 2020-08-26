using HarmonyLib;
using PowerOrder.Configuration;
using QModManager.API.ModLoading;
using QModManager.Utility;
using SMLHelper.V2.Handlers;
using System.Reflection;

namespace PowerOrder
{
    [QModCore]
    public partial class Main
    {
        internal static Config config = new Config();
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