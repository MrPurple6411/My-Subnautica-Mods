namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(NoCostConsoleCommand), nameof(NoCostConsoleCommand.Awake))]
    public static class NoCostConsoleCommand_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(NoCostConsoleCommand __instance)
        {
            if(Main.SmcConfig.FastBuild)
                __instance.fastBuildCheat = true;

            if(Main.SmcConfig.FastGrow)
                __instance.fastGrowCheat = true;

            if(Main.SmcConfig.FastHatch)
                __instance.fastHatchCheat = true;

            if(Main.SmcConfig.FastScan)
                __instance.fastScanCheat = true;
        }
    }
}
