using System.Reflection;
using UnityEngine;
using SMLHelper.V2.Handlers;
using Harmony;
using System;
using SeamothThermal.Modules;
using System.IO;
using System.Text.RegularExpressions;

namespace SeamothThermal
{
    public class Main
    {
        public static AnimationCurve ExosuitThermalReactorCharge;

        private static MethodInfo GetArmPrefabMethod =
            typeof(Exosuit).GetMethod("GetArmPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Patch()
        {
            try
            {
                var harmony = HarmonyInstance.Create("MrPurple6411.SeamothThermal");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                var exosuit = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit").GetComponent<Exosuit>();
                ExosuitThermalReactorCharge = exosuit.thermalReactorCharge;

                PrefabHandler.RegisterPrefab(new SeamothThermalModule());

                Console.WriteLine("[SeamothThermal] Succesfully patched!");
            }
            catch(Exception e)
            {
                Console.WriteLine("[SeamothThermal] Caught exception! " + e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
