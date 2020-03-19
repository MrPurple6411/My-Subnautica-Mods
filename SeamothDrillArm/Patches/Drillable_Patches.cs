using Harmony;
using SeamothDrillArm.MonoBehaviours;
using UnityEngine;

namespace SeamothDrillArm.Patches
{
    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("Start")]
    public class Drillable_Start_Patch
    {
        static void Prefix(Drillable __instance)
        {
            // Add BetterDrillable component to the Drillable GameObject
            var betterDrillable = __instance.gameObject.AddComponent<BetterDrillable>();
            betterDrillable.drillable = __instance;

            // Set the fields
            betterDrillable.resources = __instance.resources;
            betterDrillable.breakFX = __instance.breakFX;
            betterDrillable.breakAllFX = __instance.breakAllFX;
            betterDrillable.primaryTooltip = __instance.primaryTooltip;
            betterDrillable.secondaryTooltip = __instance.secondaryTooltip;
            betterDrillable.deleteWhenDrilled = __instance.deleteWhenDrilled;
            betterDrillable.modelRoot = __instance.modelRoot;
            betterDrillable.minResourcesToSpawn = __instance.minResourcesToSpawn;
            betterDrillable.maxResourcesToSpawn = __instance.maxResourcesToSpawn;
            betterDrillable.lootPinataOnSpawn = __instance.lootPinataOnSpawn;
            betterDrillable.health = __instance.health;
        }
    }

    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("OnDrill")]
    public class Drillable_OnDrill_Patch
    {
        static bool Prefix(Drillable __instance, Vector3 position, Exosuit exo, out GameObject hitObject)
        {
            // Call the BetterDrillable.OnDrill function
            var betterDrillable = __instance.GetComponent<BetterDrillable>();
            betterDrillable.OnDrill(position, exo, out hitObject);

            // Return out of original method
            return false;
        }
    }

    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("HoverDrillable")]
    public class Drillable_HoverDrillable_Patch
    {
        static bool Prefix(Drillable __instance)
        {
            // Call the BetterDrillable.HoverDrillable function
            var betterDrillable = __instance.GetComponent<BetterDrillable>();
            betterDrillable.HoverDrillable();

            // Return out of original method.
            return false;
        }
    }

    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("Restore")]
    public class Drillable_Restore_Patch
    {
        static bool Prefix(Drillable __instance)
        {
            // Call the BetterDrillable.Restore method
            var betterDrillable = __instance.GetComponent<BetterDrillable>();
            betterDrillable.Restore();

            // Return out of original method.
            return false;
        }
    }

    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("ManagedUpdate")]
    public class Drillable_ManagedUpdate_Patch
    {
        static bool Prefix(Drillable __instance)
        {
            // Call the BetterDrillable.ManagedUpdate method
            var betterDrillable = __instance.GetComponent<BetterDrillable>();
            betterDrillable.ManagedUpdate();

            // Return out of original method.
            return false;
        }
    }

    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("DestroySelf")]
    public class Drillable_DestroySelf_Patch
    {
        static bool Prefix(Drillable __instance)
        {
            // Call the BetterDrillable.ManagedUpdate method
            var betterDrillable = __instance.GetComponent<BetterDrillable>();
            betterDrillable.DestroySelf();

            // Return out of original method.
            return false;
        }
    }
}
