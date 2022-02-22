using BepInEx;
using BepInEx.Logging;

namespace PowerOrder
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Utility;
    using SMCLib.Handlers;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        internal static Config config = new();
        internal static uGUI_OptionsPanel optionsPanel;
        internal static ManualLogSource logSource;
        
        public void  Awake()
        {
            logSource = Logger;
            OptionsPanelHandler.RegisterModOptions(new Options());
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "subnautica.powerorder.mod");
            Logger.LogInfo("Patching complete.");
        }
    }
}