using System.Reflection;
using UnityEngine;
using SMLHelper.V2.Handlers;
using Harmony;
using System;
using SeamothClawArm.Modules;

namespace SeamothClawArm
{
    public class Main
    {
        public static AnimationCurve ExosuitThermalReactorCharge;

        public static void Patch()
        {
            try
            {
                var harmony = HarmonyInstance.Create("MrPurple6411.SeamothClawArm");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                PrefabHandler.RegisterPrefab(new SeamothClawModule());

                Console.WriteLine("[SeamothClawArm] Succesfully patched!");
            }
            catch(Exception e)
            {
                Console.WriteLine("[SeamothClawArm] Caught exception! " + e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
