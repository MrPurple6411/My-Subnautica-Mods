namespace PersistentCommands.Configuration;

using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using System;

[Menu("Persistent Commands")]
public class SMLConfig : ConfigFile
{
	private bool confirmed = false;

	[Button("Reset Achievements", Order = 0)]
	public void ResetAchievements(ButtonClickedEventArgs e)
	{
		if (confirmed)
		{
			confirmed = false;
			ErrorMessage.AddMessage("Achievements have been reset.");
			PlatformUtils.main.GetServices().ResetAchievements();
		}
		confirmed = true;
		ErrorMessage.AddMessage("Press again to reset Achievements.");
	}

	[Toggle(Id = "NoCost", Label = "Free Crafting"), OnChange(nameof(ToggleCheat))]
	public bool NoCost;

	[Toggle(Id = "NoBlueprints", Label = "Blueprints not required"), OnChange(nameof(ToggleCheat))]
	public bool NoBlueprints;

	[Toggle(Id = "NoEnergy", Label = "No Energy Cost"), OnChange(nameof(ToggleCheat))]
	public bool NoEnergy;

	[Toggle(Id = "NoPressure", Label = "No Crush Damage"), OnChange(nameof(ToggleCheat))]
	public bool NoPressure;

	[Toggle(Id = "NoOxygen", Label = "Oxygen Not Required"), OnChange(nameof(ToggleCheat))]
	public bool NoOxygen;

#if BELOWZERO
	[Toggle(Id = "NoCold", Label = "No Cold"), OnChange(nameof(ToggleCheat))]
	public bool NoCold;

#endif
	[Toggle(Id = "NoAggression", Label = "Creature Aggression"), OnChange(nameof(ToggleCheat))]
	public bool NoAggression;

	[Toggle(Id = "NoRadiation", Label = "No Radiation"), OnChange(nameof(ToggleCheat))]
	public bool NoRadiation;

	[Toggle(Id = "FastBuild", Label = "Fast Build"), OnChange(nameof(ToggleCheat))]
	public bool FastBuild;

	[Toggle(Id = "FastGrow", Label = "Fast Grow"), OnChange(nameof(ToggleCheat))]
	public bool FastGrow;

	[Toggle(Id = "FastHatch", Label = "Fast Hatching"), OnChange(nameof(ToggleCheat))]
	public bool FastHatch;

	[Toggle(Id = "FastScan", Label = "Fast Scanning"), OnChange(nameof(ToggleCheat))]
	public bool FastScan;


	private void ToggleCheat(ToggleChangedEventArgs e)
	{
		if (Player.main != null)
		{
#if SUBNAUTICA
			if(Enum.TryParse(e.Id, out GameModeOption option))
            {
                if(e.Value)
                {
                    GameModeUtils.ActivateCheat(option);
                }
                else
                {
                    GameModeUtils.DeactivateCheat(option);
                }
            }
            else 
#elif BELOWZERO
			switch (e.Id)
			{
				case "NoCold":
					GameModeManager.SetOption<bool>(GameOption.BodyTemperatureDecreases, !e.Value);
					GameModeManager.SetOption<bool>(GameOption.ShowTemperatureAlerts, !e.Value);
					break;

				case "NoAggression":
					GameModeManager.SetOption<float>(GameOption.CreatureAggressionModifier, e.Value ? 1f : 0f);
					EcoTarget component = Player.main?.GetComponent<EcoTarget>();
					if (component != null)
					{
						component.enabled = !e.Value;
					}
					break;

				case "NoBlueprints":
					GameModeManager.SetOption<bool>(GameOption.TechRequiresUnlocking, !e.Value);
					break;

				case "NoCost":
					GameModeManager.SetOption<bool>(GameOption.CraftingRequiresResources, !e.Value);
					break;

				case "NoEnergy":
					GameModeManager.SetOption<bool>(GameOption.TechnologyRequiresPower, !e.Value);
					break;

				case "NoOxygen":
					GameModeManager.SetOption<bool>(GameOption.OxygenDepletes, !e.Value);
					GameModeManager.SetOption<bool>(GameOption.ShowOxygenAlerts, !e.Value);
					break;

				case "NoPressure":
					GameModeManager.SetOption<bool>(GameOption.BaseWaterPressureDamage, !e.Value);
					GameModeManager.SetOption<bool>(GameOption.VehicleWaterPressureDamage, !e.Value);
					break;

				case "NoRadiation":
					GameModeManager.SetOption<bool>(GameOption.GameHasRadiationSources, !e.Value);
					break;
			}
#endif
			if (NoCostConsoleCommand.main != null)
			{
				switch (e.Id)
				{
					case "FastBuild":
						NoCostConsoleCommand.main.fastBuildCheat = e.Value;
						break;
					case "FastGrow":
						NoCostConsoleCommand.main.fastGrowCheat = e.Value;
						break;
					case "FastHatch":
						NoCostConsoleCommand.main.fastHatchCheat = e.Value;
						break;
					case "FastScan":
						NoCostConsoleCommand.main.fastScanCheat = e.Value;
						break;
				}
			}
		}
	}
}
