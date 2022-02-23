namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Submit))]
    public static class DevConsole_Submit_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if(Main.SmcConfig.NoAggression && !GameModeUtils.IsCheatActive(GameModeOption.NoAggression))
                GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

            if(Main.SmcConfig.NoBlueprints && !GameModeUtils.IsCheatActive(GameModeOption.NoBlueprints))
                GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

            if(Main.SmcConfig.NoCost && !GameModeUtils.IsCheatActive(GameModeOption.NoCost))
                GameModeUtils.ActivateCheat(GameModeOption.NoCost);

            if(Main.SmcConfig.NoEnergy && !GameModeUtils.IsCheatActive(GameModeOption.NoEnergy))
                GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

            if(Main.SmcConfig.NoOxygen && !GameModeUtils.IsCheatActive(GameModeOption.NoOxygen))
                GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

            if(Main.SmcConfig.NoPressure && !GameModeUtils.IsCheatActive(GameModeOption.NoPressure))
                GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

            if(Main.SmcConfig.NoRadiation && !GameModeUtils.IsCheatActive(GameModeOption.NoRadiation))
                GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BZ
            if (Main.SmcConfig.NoCold && !GameModeUtils.IsCheatActive(GameModeOption.NoCold))
                GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

            if(NoCostConsoleCommand.main != null)
            {
                if(Main.SmcConfig.FastBuild)
                    NoCostConsoleCommand.main.fastBuildCheat = true;

                if(Main.SmcConfig.FastGrow)
                    NoCostConsoleCommand.main.fastGrowCheat = true;

                if(Main.SmcConfig.FastHatch)
                    NoCostConsoleCommand.main.fastHatchCheat = true;

                if(Main.SmcConfig.FastScan)
                    NoCostConsoleCommand.main.fastScanCheat = true;
            }
        }

    }
}
