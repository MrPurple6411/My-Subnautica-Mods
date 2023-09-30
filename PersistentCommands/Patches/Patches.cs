namespace PersistentCommands.Patches;

using HarmonyLib;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Submit))]
    [HarmonyPostfix]
    public static void SubmitPostfix()
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
#if BELOWZERO
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

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.HasUsedConsole))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void SubmitPostfix(ref bool __result)
    {
        __result = false;
    }

    [HarmonyPatch(typeof(GameModeUtils), nameof(GameModeUtils.AllowsAchievements))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void AllowsAchievementsPostfix(ref bool __result)
    {
        __result = true;
    }

    [HarmonyPatch(typeof(NoCostConsoleCommand), nameof(NoCostConsoleCommand.Awake))]
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

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPostfix]
    public static void Postfix(Player __instance)
    {
        if(Main.SMLConfig.NoAggression)
            GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

        if(Main.SMLConfig.NoBlueprints)
            GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

        if(Main.SMLConfig.NoCost)
            GameModeUtils.ActivateCheat(GameModeOption.NoCost);

        if(Main.SMLConfig.NoEnergy)
            GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

        if(Main.SMLConfig.NoOxygen)
            GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

        if(Main.SMLConfig.NoPressure)
            GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

        if(Main.SMLConfig.NoRadiation)
            GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BELOWZERO
        if (Main.Config.NoCold)
            GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

    }

}
