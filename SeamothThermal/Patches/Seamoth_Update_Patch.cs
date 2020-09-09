using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace SeamothThermal.Patches
{
    [HarmonyPatch(typeof(SeaMoth), "Update")]
    public class Seamoth_Update_Patch
    {
        // Reflection Method: AddEnergy
        static MethodInfo AddEnergyMethod = 
            typeof(Vehicle).GetMethod("AddEnergy", BindingFlags.NonPublic | BindingFlags.Instance);

        static AnimationCurve ExosuitThermalReactorCharge { get; } = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit").GetComponent<Exosuit>().thermalReactorCharge;

        static void Prefix(SeaMoth __instance)
        {
            // If we have the SeamothThermalModule equipped.
            var count = __instance.modules.GetCount(Main.thermalModule.TechType);
            if (count > 0)
            {
                // Evaluate the energy to add based on temperature
                var temperature = __instance.GetTemperature();
                var energyToAdd = ExosuitThermalReactorCharge.Evaluate(temperature);

                // Add the energy by invoking private method using Reflection.
                AddEnergyMethod.Invoke(__instance, new object[] { energyToAdd * Time.deltaTime });
            }
        }
    }
}
