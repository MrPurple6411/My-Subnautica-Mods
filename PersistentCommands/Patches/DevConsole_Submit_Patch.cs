using HarmonyLib;
using UnityEngine;

namespace PersistentCommands.Patches
{
    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Submit))]
    public static class DevConsole_Submit_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (Main.config.NoAggression && !GameModeUtils.IsCheatActive(GameModeOption.NoAggression))
                GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

            if (Main.config.NoBlueprints && !GameModeUtils.IsCheatActive(GameModeOption.NoBlueprints))
                GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

            if (Main.config.NoCost && !GameModeUtils.IsCheatActive(GameModeOption.NoCost))
                GameModeUtils.ActivateCheat(GameModeOption.NoCost);

            if (Main.config.NoEnergy && !GameModeUtils.IsCheatActive(GameModeOption.NoEnergy))
                GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

            if (Main.config.NoOxygen && !GameModeUtils.IsCheatActive(GameModeOption.NoOxygen))
                GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

            if (Main.config.NoPressure && !GameModeUtils.IsCheatActive(GameModeOption.NoPressure))
                GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

            if (Main.config.NoRadiation && !GameModeUtils.IsCheatActive(GameModeOption.NoRadiation))
                GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BZ
            if (Main.config.NoCold && !GameModeUtils.IsCheatActive(GameModeOption.NoCold))
                GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

            if(NoCostConsoleCommand.main != null)
            {
                if (Main.config.FastBuild)
                    NoCostConsoleCommand.main.fastBuildCheat = true;

                if (Main.config.FastGrow)
                    NoCostConsoleCommand.main.fastGrowCheat = true;

                if (Main.config.FastHatch)
                    NoCostConsoleCommand.main.fastHatchCheat = true;

                if (Main.config.FastScan)
                    NoCostConsoleCommand.main.fastScanCheat = true;
            }
        }

    }
}
