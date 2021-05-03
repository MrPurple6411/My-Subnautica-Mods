#if SN1
namespace BuilderModule.Patches
{
    using BuilderModule.Module;
    using HarmonyLib;

    [HarmonyPatch(typeof(ToggleLights), nameof(ToggleLights.SetLightsActive))]
    internal class Seamoth_CheckLightToggle_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(ToggleLights __instance)
        {
            if(!Player.main.inSeamoth)
                return true;

            SeaMoth seaMoth = __instance.GetComponentInParent<SeaMoth>();

            return seaMoth is null || seaMoth != (Player.main.currentMountedVehicle as SeaMoth) || !seaMoth.TryGetComponent(out BuilderModuleMono moduleMono) || !moduleMono.isActive;
        }
    }
}
#endif