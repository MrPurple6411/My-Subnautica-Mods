using HarmonyLib;
using QModManager.Utility;

namespace BuilderModule
{
    [HarmonyPatch(typeof(Vehicle), "OnUpgradeModuleChange")]
    internal class Vehicle_OnUpgradeModuleChange_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Vehicle __instance, int slotID, TechType techType, bool added)
        {
            if (techType == BuilderModulePrefab.TechTypeID && added)
            {
                if (__instance.GetType() == typeof(SeaMoth))
                {
                    BuilderModule seamoth_control = __instance.gameObject.GetOrAddComponent<BuilderModule>();
                    seamoth_control.ModuleSlotID = slotID;
                    return;
                }
                else if (__instance.GetType() == typeof(Exosuit))
                {
                    BuilderModule exosuit_control = __instance.gameObject.GetOrAddComponent<BuilderModule>();
                    exosuit_control.ModuleSlotID = slotID;
                    return;
                }
                else
                {
                    Logger.Log(Logger.Level.Error,"Unidentified Vehicle Type!");
                }
            }
        }
    }

}
