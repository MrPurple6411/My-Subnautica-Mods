using HarmonyLib;
using UnityEngine;

namespace PersistentCommands.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Player_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (Main.config.NoAggression)
                GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

            if (Main.config.NoBlueprints)
                GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

            if (Main.config.NoCost)
                GameModeUtils.ActivateCheat(GameModeOption.NoCost);

            if (Main.config.NoEnergy)
                GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

            if (Main.config.NoOxygen)
                GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

            if (Main.config.NoPressure)
                GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

            if (Main.config.NoRadiation)
                GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BZ
            if (Main.config.NoCold)
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
