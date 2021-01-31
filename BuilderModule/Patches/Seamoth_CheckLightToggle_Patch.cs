using BuilderModule.Module;
using HarmonyLib;

namespace BuilderModule.Patches
{
    [HarmonyPatch(typeof(ToggleLights), nameof(ToggleLights.SetLightsActive))]
    class Seamoth_CheckLightToggle_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ToggleLights __instance)
        {
            if(!Player.main.inSeamoth)
                return true;

            SeaMoth seaMoth = __instance.GetComponentInParent<SeaMoth>();

            if (seaMoth is null || seaMoth != (Player.main.currentMountedVehicle as SeaMoth) || !seaMoth.TryGetComponent(out BuilderModuleMono moduleMono) || !moduleMono.isActive) 
                return true;

            return false;
        }
    }
}
