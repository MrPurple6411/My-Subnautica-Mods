using HarmonyLib;
using UnityEngine;

namespace PersistentCommands.Patches
{
    [HarmonyPatch(typeof(DevConsole), "Submit")]
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

            if (Main.config.FastBuild)
                Traverse.Create(NoCostConsoleCommand.main).Property<bool>("fastBuildCheat").Value = true;

            if (Main.config.FastGrow)
                Traverse.Create(NoCostConsoleCommand.main).Property<bool>("fastGrowCheat").Value = true;

            if (Main.config.FastHatch)
                Traverse.Create(NoCostConsoleCommand.main).Property<bool>("fastHatchCheat").Value = true;

            if (Main.config.FastScan)
                Traverse.Create(NoCostConsoleCommand.main).Property<bool>("fastScanCheat").Value = true;

        }

    }
}
