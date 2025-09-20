namespace CopperFromScanning.Configuration;

using Nautilus.Json;
using Nautilus.Options.Attributes;

// Nautilus will auto-generate an options menu and persist values to a JSON config file
[Menu("Copper From Scanning", SaveOn = MenuAttribute.SaveEvents.ChangeValue, LoadOn = MenuAttribute.LoadEvents.MenuRegistered)]
public class Config : ConfigFile
{
	// False by default: when true, the mod will not modify the scan reward (i.e., behave like vanilla)
	[Toggle("Disable mod effect", Tooltip = "When ON, this mod will not change the scan reward.")]
	public bool DisableModEffect { get; set; } = false;

	// False by default: when true, scanning fragments will not grant resources when this mod would apply
	[Toggle("Disable scanning resource rewards", Tooltip = "When ON, fragments that would grant Titanium/Copper via this mod will grant nothing instead.")]
	public bool DisableScanningResourceRewards { get; set; } = false;
}
