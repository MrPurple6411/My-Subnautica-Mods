namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(NoCostConsoleCommand), nameof(NoCostConsoleCommand.Awake))]
    public static class NoCostConsoleCommand_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(NoCostConsoleCommand __instance)
        {
            if(Main.SMLConfig.FastBuild)
                __instance.fastBuildCheat = true;

            if(Main.SMLConfig.FastGrow)
                __instance.fastGrowCheat = true;

            if(Main.SMLConfig.FastHatch)
                __instance.fastHatchCheat = true;

            if(Main.SMLConfig.FastScan)
                __instance.fastScanCheat = true;
        }
    }
}
