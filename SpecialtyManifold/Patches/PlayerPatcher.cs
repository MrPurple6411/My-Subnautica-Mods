namespace SpecialtyManifold.Patches;

using HarmonyLib;
using Nautilus.Handlers;
using SpecialtyManifold.Configuration;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HarmonyPatch(typeof(Player), nameof(Player.Update))]
internal static class PlayerPatcher
{
	private const float UpdateInterval = 3.0f;
	private const float MaxDepth = 200f;
	private const float MinTemp = 30f;

	public static Config Config { get; internal set; }
	private static TechType _scubaManifold = TechType.None;
	private static float _nextUpdate;
	private static bool _modsCheck = true;
	private static bool _modFound = false;
	private static DayNightCycle _dayNightCycle;
	private static OxygenManager _oxygenManager;
	private static WaterTemperatureSimulation _waterTemperature;

	private static string[] _photosynthesistanks = new string[]
	{
		"photosynthesistanksmall",
		"deathrunremade_photosynthesistanksmall",
		"photosynthesistank",
		"deathrunremade_photosynthesistank",
		"lwuhcptank",
	};

	private static string[] _chemosynthesistanks = new string[]
	{
		"chemosynthesistank",
		"deathrunremade_chemosynthesistank",
		"lwuhcctank"
	};

	private static HashSet<TechType> _foundPhotosynthesisTanks = new HashSet<TechType>();
	private static HashSet<TechType> _foundChemosynthesisTanks = new HashSet<TechType>();

	[HarmonyPostfix]
	public static void Postfix()
	{
		//deathrunremade_
		if (_modsCheck)
		{
			_modsCheck = false;
			_modFound = TechTypeExtensions.FromString("ScubaManifold", out _scubaManifold, true);

			foreach (var tank in _photosynthesistanks)
			{
				if (TechTypeExtensions.FromString(tank, out var techType, true))
				{
					_foundPhotosynthesisTanks.Add(techType);
				}
			}

			foreach (var tank in _chemosynthesistanks)
			{
				if (TechTypeExtensions.FromString(tank, out var techType, true))
				{
					_foundChemosynthesisTanks.Add(techType);
				}
			}

			if (_modFound && _foundChemosynthesisTanks.Count > 0 || _foundPhotosynthesisTanks.Count > 0)
			{
				Main.Log.LogDebug(_foundChemosynthesisTanks.Count > 0 ? $"SpecialtyManifold: Found ChemosynthesisTanks: {string.Join(", ", _foundChemosynthesisTanks.Select(t=>t.AsString()))}" : "SpecialtyManifold: Did not find ChemosynthesisTanks.");	
				Main.Log.LogDebug(_foundPhotosynthesisTanks.Count > 0 ? $"SpecialtyManifold: Found PhotosynthesisTanks: {string.Join(", ", _foundPhotosynthesisTanks.Select(t=>t.AsString()))}" : "SpecialtyManifold: Did not find PhotosynthesisTanks.");
				Config = OptionsPanelHandler.RegisterModOptions<Config>();
			}
			else
			{
				Main.Log.LogDebug("SpecialtyManifold: Did not find ScubaManifold or PhotosynthesisTanks or ChemosynthesisTanks.");
				Main.Log.LogDebug("SpecialtyManifold: Disabling.");
				_modFound = false;
			}
		}

		if (!_modFound)
			return;

		if (Time.time < _nextUpdate)
			return;

		_nextUpdate = Time.time + UpdateInterval;
		var tankSlot = Inventory.main.equipment.GetTechTypeInSlot("Tank");
		if (
#if SUBNAUTICA
			!GameModeUtils.RequiresOxygen() ||
#endif
			tankSlot != _scubaManifold)
			return;

		int photosynthesisTanks = 0;
		if (_foundPhotosynthesisTanks.Count > 0)
		{
			foreach (var tank in _foundPhotosynthesisTanks)
			{
				photosynthesisTanks += GetCount(tank, "lwuhcptank");
			}
		}

		UpdatePhotoSynthesisTanks(photosynthesisTanks);

		int chemosynthesisTanks = 0;
		if (_foundChemosynthesisTanks.Count > 0)
		{
			foreach (var tank in _foundChemosynthesisTanks)
			{
				chemosynthesisTanks += GetCount(tank, "lwuhcctank");
			}
		}
		UpdateChemosynthesisTanks(chemosynthesisTanks);
	}

	private static int GetCount(TechType tank, string classId)
	{
		var count = 0;

		if (Inventory.main.container._items.TryGetValue(tank, out var items))
		{
			foreach (var item in items.items)
			{
				PrefabIdentifier prefabIdentifier = item.item?.GetComponent<PrefabIdentifier>();
				if (prefabIdentifier != null && prefabIdentifier.ClassId == classId)
				{
					count += 4;
					continue;
				}

				count++;
			}
		}

		return count;
	}

	private static void UpdateChemosynthesisTanks(int chemosynthesisTanks)
	{
		if (chemosynthesisTanks <= 0)
			return;

		if (_waterTemperature == null)
			_waterTemperature = WaterTemperatureSimulation.main;

		if (_waterTemperature == null) return;

		if (_oxygenManager == null)
			_oxygenManager = Player.main.oxygenMgr;

		if (_oxygenManager == null) return;

		float temperature = _waterTemperature.GetTemperature(Player.main.transform.position);

		if (temperature < MinTemp)
			return;

		var chemosynthesisTempCalc = (UpdateInterval * temperature * 0.01f) * (Config.multipleTanks ? chemosynthesisTanks: 1);
		Player.main.oxygenMgr.AddOxygen(chemosynthesisTempCalc);
	}

	private static void UpdatePhotoSynthesisTanks(int photosynthesisTanks)
	{
		if (photosynthesisTanks <= 0)
			return;

		if (_dayNightCycle == null)
			_dayNightCycle = DayNightCycle.main;

		if (_dayNightCycle == null) return;

		if (_oxygenManager == null)
			_oxygenManager = Player.main.oxygenMgr;

		if (_oxygenManager == null) return;

		float brightness = _dayNightCycle.GetLocalLightScalar();
		float depth = Player.main.GetDepth();
		if (depth > MaxDepth)
			return; 
		
		var photosynthesisLightCalc = (UpdateInterval * brightness * ((200f - depth) / 200f)) * (Config.multipleTanks ? photosynthesisTanks: 1);	
		
		Player.main.oxygenMgr.AddOxygen(photosynthesisLightCalc);
	}
}