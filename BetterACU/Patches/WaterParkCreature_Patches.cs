namespace BetterACU.Patches;

using BetterACU.Configuration;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.BornAsync))]
internal class WaterParkCreature_Born_Prefix
{
    [HarmonyPrefix]
    public static bool Prefix(WaterPark waterPark, Vector3 position)
    {
        return waterPark.IsPointInside(position);
    }
}

[HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.Start))]
internal class WaterParkCreature_Start_Prefix
{
	[HarmonyPrefix]
	public static void Prefix(WaterParkCreature __instance)
	{
		var parkCreatureTechType = __instance.GetTechType();
		if (parkCreatureTechType == TechType.None) return;

		var creatureTechString = parkCreatureTechType.AsString();
		if (!Config.OceanBreedWhiteList.TryGetValue(creatureTechString, out int limit))
		{
			if (Config.OptionsMenu.Options.Count == 0)
				OptionsPanelHandler.RegisterModOptions(Config.OptionsMenu);

			if (!Config.OptionsMenu.Options.Any(option => option.Id == creatureTechString))
			{
				limit = 0;
				var option = ModSliderOption.Create(creatureTechString, creatureTechString, 0, 100, limit, limit);
				option.OnChanged += (sender, args) => Config.OceanBreedWhiteList[creatureTechString] = Mathf.CeilToInt(args.Value);
				Config.OceanBreedWhiteList[creatureTechString] = limit;
				Config.Instance.oceanBreedWhiteList = Config.OceanBreedWhiteList.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
				Config.Instance.Save();
				Config.OptionsMenu.AddItem(option);
			}
		}
	}
}

[HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.ManagedUpdate))]
internal class WaterParkCreatureUpdatePrefix
{
    private static readonly Dictionary<WaterParkCreature, float> TimeLastGenerated = new();

    [HarmonyPrefix]
    public static void Prefix(WaterParkCreature __instance)
    {
        if (!Config.EnablePowerGeneration) return;
        
        if (!__instance.gameObject.TryGetComponent(out LiveMixin liveMixin) || !liveMixin.IsAlive()) return;

        var techType = __instance.pickupable?.GetTechType() ?? TechType.None;

        if (!Config.CreaturePowerGeneration.TryGetValue(techType.ToString(), out var powerValue)) return;

        if(!TimeLastGenerated.TryGetValue(__instance, out var time))
        {
            time = DayNightCycle.main.timePassedAsFloat;
        }

        var power = powerValue/10 * (DayNightCycle.main.timePassedAsFloat - time) * Config.PowerGenSpeed;
        var powerSource = __instance.GetWaterPark() != null ? __instance.GetWaterPark().gameObject.GetComponent<PowerSource>() : null;
        
        if(powerSource != null)
        {
            if(!powerSource.AddEnergy(power, out var amountStored) && powerSource.connectedRelay != null)
                powerSource.connectedRelay.AddEnergy(power - amountStored, out _);
        }

        TimeLastGenerated[__instance] = DayNightCycle.main.timePassedAsFloat;
    }
}
