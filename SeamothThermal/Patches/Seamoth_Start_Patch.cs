#if SN1
namespace SeamothThermal.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Start))]
    public class Seamoth_Start_Patch
    {
        private static void Prefix(SeaMoth __instance)
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
}
#endif