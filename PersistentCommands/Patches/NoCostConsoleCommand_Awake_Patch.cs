namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(NoCostConsoleCommand), nameof(NoCostConsoleCommand.Awake))]
    public static class NoCostConsoleCommand_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(NoCostConsoleCommand __instance)
        {
            if(Main.Config.FastBuild)
                __instance.fastBuildCheat = true;

            if(Main.Config.FastGrow)
                __instance.fastGrowCheat = true;

            if(Main.Config.FastHatch)
                __instance.fastHatchCheat = true;

            if(Main.Config.FastScan)
                __instance.fastScanCheat = true;
        }
    }
}
