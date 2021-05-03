#if SN1
namespace MoreSeamothDepth
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        internal static Modules.SeamothHullModule4 moduleMK4 = new Modules.SeamothHullModule4();
        internal static Modules.SeamothHullModule5 moduleMK5 = new Modules.SeamothHullModule5();

        [QModPatch]
        public static void Load()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

                LanguageHandler.SetLanguageLine("Tooltip_VehicleHullModule3", "Enhances diving depth. Does not stack"); // To update conflicts about the maximity.

                moduleMK4.Patch();
                moduleMK5.Patch();

                Console.WriteLine("[MoreSeamothDepth] Succesfully patched!");
            }

            catch(Exception e)
            {
                Console.WriteLine("[MoreSeamothDepth] Caught exception! " + e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
#endif