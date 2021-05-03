namespace BuilderModule.Patches
{
    using BuilderModule.Module;
    using HarmonyLib;
    using QModManager.Utility;

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnUpgradeModuleChange))]
    internal class Vehicle_OnUpgradeModuleChange_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Vehicle __instance, int slotID, TechType techType, bool added)
        {
            if(techType == Main.buildermodule.TechType && added)
            {
                if(__instance.GetType() == typeof(Exosuit))
                {
                    BuilderModuleMono exosuit_control = __instance.gameObject.EnsureComponent<BuilderModuleMono>();
                    exosuit_control.ModuleSlotID = slotID;
                    return;
                }

#if SN1
                else if(__instance.GetType() == typeof(SeaMoth))
                {
                    BuilderModuleMono seamoth_control = __instance.gameObject.EnsureComponent<BuilderModuleMono>();
                    seamoth_control.ModuleSlotID = slotID;
                    return;
                }
#endif
                else
                {
                    Logger.Log(Logger.Level.Error, "Unidentified Vehicle Type!");
                }
            }
        }
    }

}
