namespace BuilderModule.Patches
{
    using Module;
    using HarmonyLib;
    using System.Collections.Generic;

    [HarmonyPatch]
    internal class ExoSuit_Patches
    {
        private static readonly Dictionary<Exosuit, BuilderModuleMono> Exosuits = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.SlotLeftDown))]
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.SlotLeftHeld))]
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.SlotLeftUp))]
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.SlotRightDown))]
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.SlotRightHeld))]
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.SlotRightUp))]
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.SlotNext))]
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.SlotPrevious))]
        private static bool Prefix(Exosuit __instance)
        {
            if (Exosuits.TryGetValue(__instance, out var moduleMono)) return !moduleMono.isToggle;
            if (!__instance.TryGetComponent(out moduleMono)) return true;
            Exosuits[__instance] = moduleMono;
            return !moduleMono.isToggle;
        }
    }
}