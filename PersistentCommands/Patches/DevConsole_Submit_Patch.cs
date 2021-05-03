namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Submit))]
    public static class DevConsole_Submit_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if(Main.Config.NoAggression && !GameModeUtils.IsCheatActive(GameModeOption.NoAggression))
                GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

            if(Main.Config.NoBlueprints && !GameModeUtils.IsCheatActive(GameModeOption.NoBlueprints))
                GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

            if(Main.Config.NoCost && !GameModeUtils.IsCheatActive(GameModeOption.NoCost))
                GameModeUtils.ActivateCheat(GameModeOption.NoCost);

            if(Main.Config.NoEnergy && !GameModeUtils.IsCheatActive(GameModeOption.NoEnergy))
                GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

            if(Main.Config.NoOxygen && !GameModeUtils.IsCheatActive(GameModeOption.NoOxygen))
                GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

            if(Main.Config.NoPressure && !GameModeUtils.IsCheatActive(GameModeOption.NoPressure))
                GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

            if(Main.Config.NoRadiation && !GameModeUtils.IsCheatActive(GameModeOption.NoRadiation))
                GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BZ
            if (Main.Config.NoCold && !GameModeUtils.IsCheatActive(GameModeOption.NoCold))
                GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

            if(NoCostConsoleCommand.main != null)
            {
                if(Main.Config.FastBuild)
                    NoCostConsoleCommand.main.fastBuildCheat = true;

                if(Main.Config.FastGrow)
                    NoCostConsoleCommand.main.fastGrowCheat = true;

                if(Main.Config.FastHatch)
                    NoCostConsoleCommand.main.fastHatchCheat = true;

                if(Main.Config.FastScan)
                    NoCostConsoleCommand.main.fastScanCheat = true;
            }
        }

    }
}
