namespace BetterACU.Configuration;

using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;

[Menu(MyPluginInfo.PLUGIN_NAME, LoadOn = MenuAttribute.LoadEvents.MenuOpened | MenuAttribute.LoadEvents.MenuRegistered,
	SaveOn = MenuAttribute.SaveEvents.ChangeValue | MenuAttribute.SaveEvents.SaveGame | MenuAttribute.SaveEvents.QuitGame)]
public class Config : ConfigFile
{
	[Toggle("Allow Normal Breeding")]
	public bool allowBreedingToACU = true;

	[Toggle("Overflow into Bio-Reactors")]
	public bool bioReactorOverflow = true;

	[Slider("Alien Containment Limit", 10, 100, DefaultValue = 10, Step = 1)]
	public int waterParkSize = 10;

	[Slider("Large Room Alien Containment Limit", 20, 200, DefaultValue = 20, Step = 1)]
	public int largeWaterParkSize = 20;

	[Toggle("Enable Power Generation")]
	public bool enablePowerGeneration = false;

	[Slider("Overall Power Generation Multiplier", 1, 100, DefaultValue = 1, Step = 1)]
	public int powerGenSpeed = 1;

	public Dictionary<string, float> creaturePowerGeneration = new() {
#if SUBNAUTICA
        { "Shocker", 2f },
		{ "CrabSquid", 1f },
		{ "GhostLeviathan", 10f },
		{ "GhostLeviathanJuvenile", 3f },
		{ "Warper", 1f }
#elif BELOWZERO
        { "Jellyfish", 2f },
		{ "SquidShark", 1f }
#endif
    };

	public Dictionary<string, int> oceanBreedWhiteList = new Dictionary<string, int>();

	public List<string> bioReactorBlackList =
#if SUBNAUTICA
		new() { "Cutefish" };
#elif BELOWZERO
		new() { "TrivalveBlue", "TrivalveYellow" };
#endif

	public Dictionary<string, float> powerValues = new();

	public static Config Instance { get; private set; }

	internal static void Register()
	{
		if (Instance != null) return;

		Instance = OptionsPanelHandler.RegisterModOptions<Config>();
		OptionsMenu = new OceanBreedingOptionsMenu();
		if (OceanBreedWhiteList.Count > 0)
		{
			OptionsPanelHandler.RegisterModOptions(OptionsMenu);
		}
		SaveUtils.RegisterOnSaveEvent(Instance.Save);
	}

	public static bool AllowBreedingToACU => Instance.allowBreedingToACU;

	public static bool BioReactorOverflow => Instance.bioReactorOverflow;

	public static int WaterParkSize => Instance.waterParkSize;

	public static int LargeWaterParkSize => Instance.largeWaterParkSize;

	public static bool EnablePowerGeneration => Instance.enablePowerGeneration;

	public static int PowerGenSpeed => Instance.powerGenSpeed;

	public static Dictionary<string, float> CreaturePowerGeneration => Instance.creaturePowerGeneration;

	public static Dictionary<string, int> OceanBreedWhiteList => Instance.oceanBreedWhiteList;

	public static List<string> BioReactorBlackList => Instance.bioReactorBlackList;

	public static Dictionary<string, float> PowerValues => Instance.powerValues;

	public static ModOptions OptionsMenu { get; private set; }

	private class OceanBreedingOptionsMenu : ModOptions
	{
		public OceanBreedingOptionsMenu() : base("Better ACU: Ocean Breeding Limits")
		{
			foreach (var creature in Config.OceanBreedWhiteList)
			{
				var option = ModSliderOption.Create(creature.Key, creature.Key, 0, 100, creature.Value, creature.Value);
				option.OnChanged += (sender, args) =>
				{
					Config.OceanBreedWhiteList[creature.Key] = Mathf.CeilToInt(args.Value);
					Config.Instance.Save();
				};
				AddItem(option);
			}
		}
	}
}
