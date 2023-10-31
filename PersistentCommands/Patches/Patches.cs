namespace PersistentCommands.Patches;

using HarmonyLib;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Submit))]
    [HarmonyPostfix]
    public static void SubmitPostfix()
    {
#if SUBNAUTICA
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
#elif BELOWZERO
		if (Player.main != null && Main.Config.NoAggression && GameModeManager.GetOption<float>(GameOption.CreatureAggressionModifier) > 0f )
		{
			Player.main.OnConsoleCommand_invisible(null);
		}

			GameModeManager.SetOption<bool>(GameOption.BodyTemperatureDecreases, Main.Config.NoCold);
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

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.HasUsedConsole))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void SubmitPostfix(ref bool __result)
    {
        __result = false;
    }

#if SUBNAUTICA
	[HarmonyPatch(typeof(GameModeUtils), nameof(GameModeUtils.AllowsAchievements))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void AllowsAchievementsPostfix(ref bool __result)
    {
        __result = true;
    }
#else
	[HarmonyPatch(typeof(GameAchievements), nameof(GameAchievements.Unlock))]
	[HarmonyPrefix]
	public static bool GetOptionPostfix(GameAchievements.Id id)
	{
		PlatformUtils.main.GetServices().UnlockAchievement(id);
		return false;
	}
#endif

	[HarmonyPatch(typeof(NoCostConsoleCommand), nameof(NoCostConsoleCommand.Awake))]
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

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPostfix]
    public static void Postfix(Player __instance)
    {
#if SUBNAUTICA
		if(Main.Config.NoAggression)
            GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

        if(Main.Config.NoBlueprints)
            GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

        if(Main.Config.NoCost)
            GameModeUtils.ActivateCheat(GameModeOption.NoCost);

        if(Main.Config.NoEnergy)
            GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

        if(Main.Config.NoOxygen)
            GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

        if(Main.Config.NoPressure)
            GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

        if(Main.Config.NoRadiation)
            GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#elif BELOWZERO
		if (Player.main != null && Main.Config.NoAggression && GameModeManager.GetOption<float>(GameOption.CreatureAggressionModifier) > 0f)
		{
			Player.main.OnConsoleCommand_invisible(null);
		}

		GameModeManager.SetOption<bool>(GameOption.TechRequiresUnlocking, Main.Config.NoBlueprints);

		if (Main.Config.NoCost)
			GameModeUtils.ActivateCheat(GameModeOption.NoCost);

		if (Main.Config.NoEnergy)
			GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

		if (Main.Config.NoOxygen)
			GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

		if (Main.Config.NoPressure)
			GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

		if (Main.Config.NoRadiation)
			GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
		if (Main.Config.NoCold)
            GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

    }

}
