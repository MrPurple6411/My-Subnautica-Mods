using Harmony;
using SeamothClawArm.MonoBehaviours;

namespace SeamothClawArm.Patches
{
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleToggle")]
    public class Seamoth_OnUpgradeModuleToggle_Patch
    {
        static void Postfix(SeaMoth __instance, int slotID, bool active)
        {
            // Find the TechType in the toggled slot.
            // Valid inputs would be along the lines of: SeamothModule1, SeamothModule2, etc
            // slotID is 0-based, so an addition of 1 is required.
            var techType = __instance.modules.GetTechTypeInSlot($"SeamothModule{slotID + 1}");

            // If its the SeamothClawModule
            if (techType == SeamothModule.SeamothClawModule)
            {
                // Get the SeamothClaw component from the SeaMoth object.
                var seamothClawModule = __instance.GetComponent<SeamothClaw>();

                // If its not null
                if (seamothClawModule != null)
                {
                    // Set its toggle!
                    seamothClawModule.toggle = active;
                }
            }
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Start")]
    public class Seamoth_Start_Patch
    {
        static void Prefix(SeaMoth __instance)
        {
            // Add the SeamothClaw component to the Seamoth on start.
            __instance.gameObject.AddComponent<SeamothClaw>();
        }
    }
}
