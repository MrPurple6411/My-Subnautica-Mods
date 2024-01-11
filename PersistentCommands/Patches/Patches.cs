namespace PersistentCommands.Patches;

using HarmonyLib;
using System.Collections;
using UnityEngine;

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
		if (Main.Config.NoAggression)
		{
			GameModeManager.SetOption<float>(GameOption.CreatureAggressionModifier, 0f);
			EcoTarget component = Player.main?.GetComponent<EcoTarget>();
			if (component != null)
			{
				component.enabled = false;
			}
			Main.Log.LogDebug("Aggression is now disabled.");
		}

		if (Main.Config.NoBlueprints)
		{
			GameModeManager.SetOption<bool>(GameOption.TechRequiresUnlocking, !Main.Config.NoBlueprints);
			Main.Log.LogDebug("Blueprints are now " + (Main.Config.NoBlueprints ? "required" : "not required") + ".");
		}

		if (Main.Config.NoCost)
		{
			GameModeManager.SetOption<bool>(GameOption.CraftingRequiresResources, !Main.Config.NoCost);
			Main.Log.LogDebug("Crafting is now " + (Main.Config.NoCost ? "free" : "not free") + ".");
		}

		if (Main.Config.NoEnergy)
		{
			GameModeManager.SetOption<bool>(GameOption.TechnologyRequiresPower, !Main.Config.NoEnergy);
			Main.Log.LogDebug("Energy is now " + (Main.Config.NoEnergy ? "free" : "not free") + ".");
		}

		if (Main.Config.NoOxygen)
		{
			GameModeManager.SetOption<bool>(GameOption.OxygenDepletes, !Main.Config.NoOxygen);
			GameModeManager.SetOption<bool>(GameOption.ShowOxygenAlerts, !Main.Config.NoOxygen);
			Main.Log.LogDebug("Oxygen is now " + (Main.Config.NoOxygen ? "free" : "not free") + ".");
		}

		if (Main.Config.NoPressure)
		{
			GameModeManager.SetOption<bool>(GameOption.BaseWaterPressureDamage, !Main.Config.NoPressure);
			GameModeManager.SetOption<bool>(GameOption.VehicleWaterPressureDamage, !Main.Config.NoPressure);
			Main.Log.LogDebug("Pressure is now " + (Main.Config.NoPressure ? "free" : "not free") + ".");
		}

		if (Main.Config.NoRadiation)
		{
			GameModeManager.SetOption<bool>(GameOption.GameHasRadiationSources, !Main.Config.NoRadiation);
			Main.Log.LogDebug("Radiation is now " + (Main.Config.NoRadiation ? "free" : "not free") + ".");
		}

		if (Main.Config.NoCold)
		{
			GameModeManager.SetOption<bool>(GameOption.BodyTemperatureDecreases, !Main.Config.NoCold);
			GameModeManager.SetOption<bool>(GameOption.ShowTemperatureAlerts, !Main.Config.NoCold);
			Main.Log.LogDebug("Cold is now " + (Main.Config.NoCold ? "disabled" : "enabled") + ".");
		}
#endif

		if (NoCostConsoleCommand.main != null)
		{
			if (Main.Config.FastBuild)
			{
				NoCostConsoleCommand.main.fastBuildCheat = true;
				Main.Log.LogDebug("Fast build is now enabled.");
			}

			if (Main.Config.FastGrow)
			{
				NoCostConsoleCommand.main.fastGrowCheat = true;
				Main.Log.LogDebug("Fast grow is now enabled.");
			}

			if (Main.Config.FastHatch)
			{
				NoCostConsoleCommand.main.fastHatchCheat = true;
				Main.Log.LogDebug("Fast hatch is now enabled.");
			}

			if (Main.Config.FastScan)
			{
				NoCostConsoleCommand.main.fastScanCheat = true;
				Main.Log.LogDebug("Fast scan is now enabled.");
			}
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
		if (Main.Config.FastBuild)
		{
			__instance.fastBuildCheat = true;
			Main.Log.LogDebug("Fast build is now enabled.");
		}

		if (Main.Config.FastGrow)
		{
			__instance.fastGrowCheat = true;
			Main.Log.LogDebug("Fast grow is now enabled.");
		}

		if (Main.Config.FastHatch)
		{
			__instance.fastHatchCheat = true;
			Main.Log.LogDebug("Fast hatch is now enabled.");
		}

		if (Main.Config.FastScan)
		{
			__instance.fastScanCheat = true;
			Main.Log.LogDebug("Fast scan is now enabled.");
		}
	}

#if SUBNAUTICA
	[HarmonyPatch(typeof(Player), nameof(Player.Awake))]
	[HarmonyPostfix]
	public static void Postfix(Player __instance)
	{
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
	}
#elif BELOWZERO

	[HarmonyPatch(typeof(GameModeManager), nameof(GameModeManager.SetGameOptions)), HarmonyPostfix]
	public static void CreateValuesManagersPostfix()
	{
		if (Main.Config.NoAggression)
		{
			GameModeManager.SetOption<float>(GameOption.CreatureAggressionModifier, 0f);
			EcoTarget component = Player.main?.GetComponent<EcoTarget>();
			if (component != null)
			{
				component.enabled = false;
			}
			Main.Log.LogDebug("Aggression is now disabled.");
		}

		if (Main.Config.NoBlueprints)
		{
			GameModeManager.SetOption<bool>(GameOption.TechRequiresUnlocking, !Main.Config.NoBlueprints);
			Main.Log.LogDebug("Blueprints are now " + (Main.Config.NoBlueprints ? "required" : "not required") + ".");
		}

		if (Main.Config.NoCost)
		{
			GameModeManager.SetOption<bool>(GameOption.CraftingRequiresResources, !Main.Config.NoCost);
			Main.Log.LogDebug("Crafting is now " + (Main.Config.NoCost ? "free" : "not free") + ".");
		}

		if (Main.Config.NoEnergy)
		{
			GameModeManager.SetOption<bool>(GameOption.TechnologyRequiresPower, !Main.Config.NoEnergy);
			Main.Log.LogDebug("Energy is now " + (Main.Config.NoEnergy ? "free" : "not free") + ".");
		}

		if (Main.Config.NoOxygen)
		{
			GameModeManager.SetOption<bool>(GameOption.OxygenDepletes, !Main.Config.NoOxygen);
			GameModeManager.SetOption<bool>(GameOption.ShowOxygenAlerts, !Main.Config.NoOxygen);
			Main.Log.LogDebug("Oxygen is now " + (Main.Config.NoOxygen ? "free" : "not free") + ".");
		}

		if (Main.Config.NoPressure)
		{
			GameModeManager.SetOption<bool>(GameOption.BaseWaterPressureDamage, !Main.Config.NoPressure);
			GameModeManager.SetOption<bool>(GameOption.VehicleWaterPressureDamage, !Main.Config.NoPressure);
			Main.Log.LogDebug("Pressure is now " + (Main.Config.NoPressure ? "free" : "not free") + ".");
		}

		if (Main.Config.NoRadiation)
		{
			GameModeManager.SetOption<bool>(GameOption.GameHasRadiationSources, !Main.Config.NoRadiation);
			Main.Log.LogDebug("Radiation is now " + (Main.Config.NoRadiation ? "free" : "not free") + ".");
		}

		if (Main.Config.NoCold)
		{
			GameModeManager.SetOption<bool>(GameOption.BodyTemperatureDecreases, !Main.Config.NoCold);
			GameModeManager.SetOption<bool>(GameOption.ShowTemperatureAlerts, !Main.Config.NoCold);
			Main.Log.LogDebug("Cold is now " + (Main.Config.NoCold ? "disabled" : "enabled") + ".");
		}
	}
#endif
}
