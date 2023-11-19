namespace PersistentCommands.Configuration;

using Microsoft.Win32;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using System;

[Menu("Persistent Commands")]
public class SMLConfig: ConfigFile
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
        confirmed= true;
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
    [Toggle(Id = "NoAggression", Label = "Invisiblity"), OnChange(nameof(ToggleCheat))]
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
        if(Player.main != null)
        {
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
            else if(NoCostConsoleCommand.main != null)
            {
                switch(e.Id)
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
