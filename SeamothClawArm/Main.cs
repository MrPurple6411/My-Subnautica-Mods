using Harmony;
using SeamothClawArm.Modules;
using SMLHelper.V2.Handlers;
using System;
using System.Reflection;
using UnityEngine;

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
            catch (Exception e)
            {
                Console.WriteLine("[SeamothClawArm] Caught exception! " + e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
