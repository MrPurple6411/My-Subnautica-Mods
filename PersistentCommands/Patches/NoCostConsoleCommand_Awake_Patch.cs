using HarmonyLib;

namespace PersistentCommands.Patches
{
    [HarmonyPatch(typeof(NoCostConsoleCommand), nameof(NoCostConsoleCommand.Awake))]
    public static class NoCostConsoleCommand_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(NoCostConsoleCommand __instance)
        {
            if (Main.config.FastBuild)
                __instance.fastBuildCheat = true;

            if (Main.config.FastGrow)
                __instance.fastGrowCheat = true;

            if (Main.config.FastHatch)
                __instance.fastHatchCheat = true;

            if (Main.config.FastScan)
                __instance.fastScanCheat = true;
        }
    }
}
