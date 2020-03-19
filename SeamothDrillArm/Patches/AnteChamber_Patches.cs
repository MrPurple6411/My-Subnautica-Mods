using Harmony;
using SeamothDrillArm.MonoBehaviours;

namespace SeamothDrillArm.Patches
{
    [HarmonyPatch(typeof(AnteChamber))]
    [HarmonyPatch("Start")]
    public class AnteChamber_Start_Patch
    {
        static void Postfix(AnteChamber __instance)
        {
            // Remove the event handler
            __instance.drillable.onDrilled -= __instance.OnDrilled;

            var betterDrillable = __instance.drillable.GetComponent<BetterDrillable>();
            if (betterDrillable == null)
            {
                // Add BetterDrillable component to the Drillable GameObject
                betterDrillable = __instance.drillable.gameObject.AddComponent<BetterDrillable>();
                betterDrillable.drillable = __instance.drillable;

                // Set the fields
                betterDrillable.resources = __instance.drillable.resources;
                betterDrillable.breakFX = __instance.drillable.breakFX;
                betterDrillable.breakAllFX = __instance.drillable.breakAllFX;
                betterDrillable.primaryTooltip = __instance.drillable.primaryTooltip;
                betterDrillable.secondaryTooltip = __instance.drillable.secondaryTooltip;
                betterDrillable.deleteWhenDrilled = __instance.drillable.deleteWhenDrilled;
                betterDrillable.modelRoot = __instance.drillable.modelRoot;
                betterDrillable.minResourcesToSpawn = __instance.drillable.minResourcesToSpawn;
                betterDrillable.maxResourcesToSpawn = __instance.drillable.maxResourcesToSpawn;
                betterDrillable.lootPinataOnSpawn = __instance.drillable.lootPinataOnSpawn;
                betterDrillable.health = __instance.drillable.health;
            }

            // Add our own handler.
            betterDrillable.onDrilled += __instance.OnDrilled;
        }
    }
}
