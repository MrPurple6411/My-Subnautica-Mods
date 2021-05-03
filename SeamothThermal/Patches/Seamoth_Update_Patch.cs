#if SN1
namespace SeamothThermal.Patches
{
    using HarmonyLib;
    using System.Reflection;
    using UnityEngine;

    [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update))]
    public class Seamoth_Update_Patch
    {
        // Reflection Method: AddEnergy
        private static MethodInfo AddEnergyMethod =
            typeof(Vehicle).GetMethod("AddEnergy", BindingFlags.NonPublic | BindingFlags.Instance);

        private static AnimationCurve ExosuitThermalReactorCharge { get; } = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit").GetComponent<Exosuit>().thermalReactorCharge;

        private static void Prefix(SeaMoth __instance)
        {
            // If we have the SeamothThermalModule equipped.
            int count = __instance.modules.GetCount(Main.thermalModule.TechType);
            if(count > 0)
            {
                // Evaluate the energy to add based on temperature
                float temperature = __instance.GetTemperature();
                float energyToAdd = ExosuitThermalReactorCharge.Evaluate(temperature);

                // Add the energy by invoking private method using Reflection.
                AddEnergyMethod.Invoke(__instance, new object[] { energyToAdd * Time.deltaTime });
            }
        }
    }
}
#endif