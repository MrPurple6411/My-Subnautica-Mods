using Harmony;
using MoreSeamothDepth.Modules;
using SMLHelper.V2.Handlers;
using System;
using System.Reflection;

namespace MoreSeamothDepth
{
    public class Main
    {
        public static void Patch()
        {
            try
            {
                var harmony = HarmonyInstance.Create("MrPurple6411.MoreSeamothDepth");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                LanguageHandler.SetLanguageLine("Tooltip_VehicleHullModule3", "Enhances diving depth. Does not stack"); // To update conflicts about the maximity.

                PrefabHandler.RegisterPrefab(new SeamothHullModule4());
                PrefabHandler.RegisterPrefab(new SeamothHullModule5());

                Console.WriteLine("[MoreSeamothDepth] Succesfully patched!");
            }
            catch (Exception e)
            {
                Console.WriteLine("[MoreSeamothDepth] Caught exception! " + e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
