using System;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace BuilderModule
{
    public static class Main
    {
        public static void Load()
        {
            try
            {
                var buildermodule = new BuilderModulePrefab();
                buildermodule.Patch();
                HarmonyInstance.Create("MrPurple6411.BuilderModule").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Vehicle_OnUpgradeModuleChange_Patch
    {
        [HarmonyPostfix]
        static void Postfix(Vehicle __instance, int slotID, TechType techType, bool added)
        {
            if (techType == BuilderModulePrefab.TechTypeID && added)
            {
                if (__instance.GetType() == typeof(SeaMoth))
                {
                    var seamoth_control = __instance.gameObject.GetOrAddComponent<BuilderModuleSeamoth>();
                    seamoth_control.moduleSlotID = slotID;
                    return;
                }
                else
                {
                    Debug.Log("[BuilderModule] Error! Unidentified Vehicle Type!");
                }
            }
        }
    }
}
