using Harmony;
using System.Reflection;
using UnityEngine;

namespace SeamothThermal.Patches
{

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Start")]
    public class Seamoth_Start_Patch
    {
        static void Prefix(SeaMoth __instance)
        {
            // Get TemperatureDamage class from SeaMoth
            var tempDamage = __instance.GetComponent<TemperatureDamage>();

            // Set the different fields
            // No need to check for depth because the SeaMoth would already 
            // be dead if you don't have the depth modules.
            tempDamage.baseDamagePerSecond = 0.2f;
            tempDamage.onlyLavaDamage = true;
            tempDamage.minDamageTemperature = 50;
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Update")]
    public class Seamoth_Update_Patch
    {
        // Reflection Method: AddEnergy
        static MethodInfo AddEnergyMethod =
            typeof(Vehicle).GetMethod("AddEnergy", BindingFlags.NonPublic | BindingFlags.Instance);

        static void Prefix(SeaMoth __instance)
        {
            // If we have the SeamothThermalModule equipped.
            var count = __instance.modules.GetCount(SeamothModule.SeamothThermalModule);
            if (count > 0)
            {
                // Evaluate the energy to add based on temperature
                var temperature = __instance.GetTemperature();
                var energyToAdd = Main.ExosuitThermalReactorCharge.Evaluate(temperature);

                // Add the energy by invoking private method using Reflection.
                AddEnergyMethod.Invoke(__instance, new object[] { energyToAdd * Time.deltaTime * (count * count) });
            }
        }
    }
}
