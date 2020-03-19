using Harmony;
using SeamothDrillArm.Modules;
using SMLHelper.V2.Handlers;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SeamothDrillArm
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
                var harmony = HarmonyInstance.Create("MrPurple6411.SeamothDrillArm");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                var exosuit = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit").GetComponent<Exosuit>();
                ExosuitThermalReactorCharge = exosuit.thermalReactorCharge;

                var exosuitDrillArmGO = (GameObject)GetArmPrefabMethod.Invoke(exosuit, new object[] { TechType.ExosuitDrillArmModule });
                var exosuitDrillArm = exosuitDrillArmGO.GetComponent<ExosuitDrillArm>();
                DrillLoopHit = exosuitDrillArm.loopHit;
                DrillLoop = exosuitDrillArm.loop;

                var path = @"./QMods/QuickMiner/mod.json";
                if (!File.Exists(path))
                {
                    Console.WriteLine("[SeamothDrillArm] Quick Miner not installed; node health set to default");
                }
                else
                {
                    Console.WriteLine("[SeamothDrillArm] Quick Miner IS installed; reading config...");
                    var qmConfigJson = File.ReadAllText(path);
                    string nodeHealthPattern = "\"NodeHealth\"\\s*:\\s*(\\d+\\.?\\d*)";
                    Match match = Regex.Match(qmConfigJson, nodeHealthPattern);
                    if (match.Success)
                    {
                        GroupCollection iAmGroup = match.Groups;
                        float qmNodeHealth = 0;
                        if (float.TryParse(iAmGroup[1].Value, out qmNodeHealth))
                        {
                            Console.WriteLine("[SeamothDrillArm] New node health is " + qmNodeHealth + ", based on QM config.");
                            DrillNodeHealth = qmNodeHealth;
                        }
                        else
                        {
                            Console.WriteLine("[SeamothDrillArm] Read QM config, but couldn't get the value! Using the default value of 50.");
                            DrillNodeHealth = 50f;
                        }
                    }
                    else
                    {
                        Console.WriteLine("[SeamothDrillArm] Couldn't find the node health config! Using the default value of 50.");
                        DrillNodeHealth = 50f;
                    }
                }

                PrefabHandler.RegisterPrefab(new SeamothDrillModule());

                Console.WriteLine("[SeamothDrillArm] Succesfully patched!");
            }
            catch (Exception e)
            {
                Console.WriteLine("[SeamothDrillArm] Caught exception! " + e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
