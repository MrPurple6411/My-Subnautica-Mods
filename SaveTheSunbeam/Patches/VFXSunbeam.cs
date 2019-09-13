using Harmony;
using System;

namespace SaveTheSunbeam
{
    [HarmonyPatch(typeof(VFXSunbeam))]
    [HarmonyPatch("Update")]
    static class VFXSunbeam_Update
    {
        [HarmonyPrefix]
        static void Prefix(VFXSunbeam __instance)
        {
            if (Mod.ship == null || !Mod.ship) return;
            if (Mod.ship.transform == null || !Mod.ship.transform) return;
            Console.WriteLine($"{Mod.ship.transform.position.x} {Mod.ship.transform.position.y} {Mod.ship.transform.position.z} | {Mod.ship.transform.rotation.x} {Mod.ship.transform.rotation.y} {Mod.ship.transform.rotation.z}");
        }
    }
}
