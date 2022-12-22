namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Submit))]
    public static class DevConsole_Submit_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if(Main.SMLConfig.NoAggression && !GameModeUtils.IsCheatActive(GameModeOption.NoAggression))
                GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

            if(Main.SMLConfig.NoBlueprints && !GameModeUtils.IsCheatActive(GameModeOption.NoBlueprints))
                GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

            if(Main.SMLConfig.NoCost && !GameModeUtils.IsCheatActive(GameModeOption.NoCost))
                GameModeUtils.ActivateCheat(GameModeOption.NoCost);

            if(Main.SMLConfig.NoEnergy && !GameModeUtils.IsCheatActive(GameModeOption.NoEnergy))
                GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

            if(Main.SMLConfig.NoOxygen && !GameModeUtils.IsCheatActive(GameModeOption.NoOxygen))
                GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

            if(Main.SMLConfig.NoPressure && !GameModeUtils.IsCheatActive(GameModeOption.NoPressure))
                GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

            if(Main.SMLConfig.NoRadiation && !GameModeUtils.IsCheatActive(GameModeOption.NoRadiation))
                GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BZ
            if (Main.Config.NoCold && !GameModeUtils.IsCheatActive(GameModeOption.NoCold))
                GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

            if(NoCostConsoleCommand.main != null)
            {
                if(Main.SMLConfig.FastBuild)
                    NoCostConsoleCommand.main.fastBuildCheat = true;

                if(Main.SMLConfig.FastGrow)
                    NoCostConsoleCommand.main.fastGrowCheat = true;

                if(Main.SMLConfig.FastHatch)
                    NoCostConsoleCommand.main.fastHatchCheat = true;

                if(Main.SMLConfig.FastScan)
                    NoCostConsoleCommand.main.fastScanCheat = true;
            }
        }

    }
}
