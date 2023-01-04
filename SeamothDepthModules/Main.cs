#if SN1
namespace MoreSeamothDepth
{
    using HarmonyLib;
    using SMLHelper.Handlers;
    using System;
    using System.Reflection;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "MoreSeamothDepth",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.1";
        internal static Modules.SeamothHullModule4 moduleMK4 = new();
        internal static Modules.SeamothHullModule5 moduleMK5 = new();
        #endregion

        private void Awake()
        {
            try
            {
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
                LanguageHandler.SetLanguageLine("Tooltip_VehicleHullModule3", "Enhances diving depth. Does not stack"); // To update conflicts about the maximity.
                moduleMK4.Patch();
                moduleMK5.Patch();
                Logger.LogInfo("Succesfully patched!");
            }
            catch(Exception e)
            {
                Logger.LogError(e.InnerException.Message);
                Logger.LogError(e.InnerException.StackTrace);
            }
        }
    }
}
#endif