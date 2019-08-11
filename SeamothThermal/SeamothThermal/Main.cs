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
        public static FMOD_CustomLoopingEmitter DrillLoop;
        public static FMOD_CustomLoopingEmitter DrillLoopHit;
        public static float DrillNodeHealth = 200f;

        private static MethodInfo GetArmPrefabMethod =
            typeof(Exosuit).GetMethod("GetArmPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Patch()
        {
            try
            {
                var harmony = HarmonyInstance.Create("MrPurple6411.SeamothThermal");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                LanguageHandler.SetLanguageLine("Tooltip_VehicleHullModule3", "Enhances diving depth. Does not stack"); // To update conflicts about the maximity.

                var exosuit = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit").GetComponent<Exosuit>();
                ExosuitThermalReactorCharge = exosuit.thermalReactorCharge;

                var exosuitDrillArmGO = (GameObject)GetArmPrefabMethod.Invoke(exosuit, new object[] { TechType.ExosuitDrillArmModule });
                var exosuitDrillArm = exosuitDrillArmGO.GetComponent<ExosuitDrillArm>();
                DrillLoopHit = exosuitDrillArm.loopHit;
                DrillLoop = exosuitDrillArm.loop;

                var path = @"./QMods/QuickMiner/mod.json";
                if (!File.Exists(path))
                {
                    Console.WriteLine("[SeamothThermal] Quick Miner not installed; node health set to default");
                }
                else
                {
                    Console.WriteLine("[SeamothThermal] Quick Miner IS installed; reading config...");
                    var qmConfigJson = File.ReadAllText(path);
                    string nodeHealthPattern = "\"NodeHealth\"\\s*:\\s*(\\d+\\.?\\d*)";
                    Match match = Regex.Match(qmConfigJson, nodeHealthPattern);
                    if (match.Success)
                    {
                        GroupCollection iAmGroup = match.Groups;
                        float qmNodeHealth = 0;
                        if (float.TryParse(iAmGroup[1].Value, out qmNodeHealth))
                        {
                            Console.WriteLine("[SeamothThermal] New node health is " + qmNodeHealth + ", based on QM config.");
                            DrillNodeHealth = qmNodeHealth;
                        }
                        else
                        {
                            Console.WriteLine("[SeamothThermal] Read QM config, but couldn't get the value! Using the default value of 50.");
                            DrillNodeHealth = 50f;
                        }
                    }
                    else
                    {
                        Console.WriteLine("[SeamothThermal] Couldn't find the node health config! Using the default value of 50.");
                        DrillNodeHealth = 50f;
                    }
                }


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
