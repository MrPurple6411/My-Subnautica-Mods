using System.Collections.Generic;
using System.Reflection;
using Harmony;
using QModManager.API.ModLoading;
using UnityEngine;

namespace EnergyMixinFiltering
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"YourName_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }

    [HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.ModifyCharge))]
    public class EnergyMixin_ModifyCharge_patch
    {
        [HarmonyPrefix]
        public static bool Prefix(EnergyMixin __instance, float amount)
        {
            List<TechType> BatteryBlacklist = new List<TechType>() { };
            GameObject currentBattery = __instance.GetBattery();


            if (amount > 0 && currentBattery != null && BatteryBlacklist.Contains(CraftData.GetTechType(currentBattery)))
            {
                return false;
            }
            return true;
        }
    }
}