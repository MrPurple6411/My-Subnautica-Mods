namespace BetterACU.Patches;

using BetterACU.Configuration;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UWE;

[HarmonyPatch(typeof(WaterPark), nameof(WaterPark.Update))]
public static class WaterParkUpdatePostfix
{
	private static readonly Dictionary<WaterPark, List<WaterParkItem>> CachedItems = new();
	private static readonly Dictionary<WaterPark, List<WaterParkItem>> CachedPowerCreatures = new();

	[HarmonyPostfix]
	public static void Postfix(WaterPark __instance)
	{
		if (!Config.EnablePowerGeneration) return;

		var powerCreatures = new List<WaterParkItem>();
		float maxPower = 0;
		if (CachedItems.TryGetValue(__instance, out var items) && items.Count == __instance.items.Count && CachedPowerCreatures.ContainsKey(__instance))
		{
			powerCreatures.AddRange(CachedPowerCreatures[__instance]);

			foreach (var creature in CachedPowerCreatures[__instance])
			{
				if (!creature.gameObject.TryGetComponent(out LiveMixin liveMixin) || !liveMixin.IsAlive())
					powerCreatures.Remove(creature);
			}

			maxPower += Config.CreaturePowerGeneration
				.Select(pair => new
				{
					pair,
					creatures = __instance.items.FindAll(item => item.pickupable.GetTechType().ToString() == pair.Key)
				})
				.Where(t => t.creatures.Count > 0)
				.Select(selector: t => 50 * t.pair.Value * t.creatures.Count).Sum();

			CachedPowerCreatures[__instance] = powerCreatures;
		}
		else
		{
			foreach (var pair in Config.CreaturePowerGeneration)
			{
				var creatures = __instance.items.FindAll(item => item.pickupable.GetTechType().ToString() == pair.Key && item.GetComponent<LiveMixin>() != null && item.GetComponent<LiveMixin>().IsAlive());
				if (creatures.Count <= 0) continue;

				maxPower += 50 * pair.Value * creatures.Count;
				powerCreatures.AddRange(creatures);
			}

			CachedItems[__instance] = __instance.items;
			CachedPowerCreatures[__instance] = powerCreatures;
		}

		var rootObject = __instance.gameObject;

		var powerSource = rootObject.GetComponent<PowerSource>();
		if (powerSource is null)
		{
			powerSource = rootObject.AddComponent<PowerSource>();
			powerSource.maxPower = maxPower;
			powerSource.power = Config.PowerValues.GetOrDefault($"PowerSource:{__instance.GetInstanceID()}", 0f);
		}
		else
		{
			powerSource.maxPower = maxPower;

			if (powerSource.power > powerSource.maxPower)
				powerSource.power = powerSource.maxPower;

			Config.PowerValues[$"PowerSource:{__instance.GetInstanceID()}"] = powerSource.power;
		}
	}
}

[HarmonyPatch(typeof(WaterPark), nameof(WaterPark.HasFreeSpace))]
internal class WaterParkHasFreeSpacePostfix
{
	[HarmonyPrefix]
	public static void Prefix(WaterPark __instance)
	{
		__instance.wpPieceCapacity = Config.WaterParkSize;
	}
}
[HarmonyPatch(typeof(LargeRoomWaterPark), nameof(LargeRoomWaterPark.HasFreeSpace))]
internal class LargeRoomWaterPark_HasFreeSpace_Postfix
{
	[HarmonyPrefix]
	public static void Prefix(WaterPark __instance)
	{
		__instance.wpPieceCapacity = Config.LargeWaterParkSize;
	}
}

[HarmonyPatch(typeof(WaterPark), nameof(WaterPark.GetBreedingPartner))]
internal class WaterParkBreedPostfix
{
	const string LoadError = "Error loading or saving ocean breed counts. Check log for details.";
	private static bool _hasLoadError = false;
	public static Dictionary<string, Dictionary<string, int>> SavedOceanBreedCounts { get; private set; }
	public static Dictionary<WaterPark, PrefabIdentifier> WaterParkIdentifiers { get; } = new Dictionary<WaterPark, PrefabIdentifier>();

	[HarmonyPrefix]
	public static bool Prefix(ref WaterParkCreature __result)
	{
		__result = null;
		return Config.AllowBreedingToACU;
	}

	[HarmonyPostfix]
	public static void Postfix(WaterPark __instance, WaterParkCreature creature)
	{
		if (!_hasLoadError && SavedOceanBreedCounts == null)
		{
			LoadSavedBreedCounts();
			if (!_hasLoadError)
			{
				Main.Logger.LogInfo("Loaded saved ocean breed counts.");
				SaveUtils.RegisterOnSaveEvent(SaveOceanBreedCounts);
			}
		}

		var items = __instance.items;
		var techType = creature.pickupable.GetTechType();

		if (!items.Contains(creature) || (__instance.HasFreeSpace() && Config.AllowBreedingToACU))
			return;

		if (!WaterParkIdentifiers.TryGetValue(__instance, out var identifier))
			identifier = WaterParkIdentifiers[__instance] = __instance.gameObject.GetComponent<PrefabIdentifier>();

		var hasBred = false;
		foreach (var waterParkItem in items)
		{
			var parkCreature = waterParkItem as WaterParkCreature;
			var parkCreatureTechType = parkCreature is not null && parkCreature.pickupable != null ? parkCreature.pickupable.GetTechType() : TechType.None;
			var creatureTechString = parkCreatureTechType.AsString();
			if (parkCreature == null || parkCreature == creature || !parkCreature.GetCanBreed() ||
				parkCreatureTechType != techType || creatureTechString.Contains("Egg"))
			{
				continue;
			}

			if (Config.BioReactorOverflow && BaseBioReactor.GetCharge(parkCreatureTechType) > -1)
			{
				if (!Config.BioReactorBlackList.Contains(creatureTechString) && !hasBred)
				{
					var baseBioReactors =
						__instance.gameObject.GetComponentInParent<SubRoot>()?.gameObject
							.GetComponentsInChildren<BaseBioReactor>()
							?.Where(baseBioReactor => baseBioReactor.container.HasRoomFor(parkCreature.pickupable))
							.ToList() ?? new List<BaseBioReactor>();

					if (baseBioReactors.Count > 0)
					{
						hasBred = true;
						baseBioReactors.Shuffle();
						var baseBioReactor = baseBioReactors.First();
						CoroutineHost.StartCoroutine(SpawnCreature(__instance, parkCreatureTechType, baseBioReactor.container));
					}
				}
			}

			if (!_hasLoadError && identifier != null)
			{
				if (!Config.OceanBreedWhiteList.TryGetValue(creatureTechString, out int limit))
				{
					limit = 0;
					var option = ModSliderOption.Create(creatureTechString, creatureTechString, 0, 100, limit, limit);
					option.OnChanged += (sender, args) => Config.OceanBreedWhiteList[creatureTechString] = Mathf.CeilToInt(args.Value);
					Config.OptionsMenu.AddItem(option);

					if (Config.OceanBreedWhiteList.Count == 0)
						OptionsPanelHandler.RegisterModOptions(Config.OptionsMenu);
				}

				Dictionary<string, int> breedCounts;
				if (!SavedOceanBreedCounts.TryGetValue(identifier.Id, out breedCounts))
					breedCounts = SavedOceanBreedCounts[identifier.Id] = new Dictionary<string, int>();

				if (!breedCounts.TryGetValue(creatureTechString, out int count))
					count = breedCounts[creatureTechString] = 0;

				if (count < limit && !hasBred && __instance.transform.position.y < 0)
				{
					hasBred = true;
					breedCounts[creatureTechString] = count + 1;
					CoroutineHost.StartCoroutine(SpawnCreature(__instance, parkCreatureTechType, null));
				}
			}

			if (hasBred)
			{
				creature.ResetBreedTime();
				parkCreature.ResetBreedTime();
			}
			break;
		}
	}

	private static void SaveOceanBreedCounts()
	{
		var savePath = SaveLoadManager.GetTemporarySavePath();
		var saveFile = Path.Combine(savePath + "oceanBreedCounts.json");
		try
		{
			var json = JsonUtility.ToJson(SavedOceanBreedCounts);
			File.WriteAllText(saveFile, json);
		}
		catch (Exception e)
		{
			_hasLoadError = true;
			ErrorMessage.AddDebug(LoadError);

			do
			{
				Main.Logger.LogError(e.Message);
				Main.Logger.LogError(e.StackTrace);
				e = e.InnerException;
			}
			while (e != null);
		}
	}

	private static void LoadSavedBreedCounts()
	{
		var savePath = SaveLoadManager.GetTemporarySavePath();
		var saveFile = Path.Combine(savePath + "oceanBreedCounts.json");
		if (!File.Exists(saveFile))
		{
			SavedOceanBreedCounts = new Dictionary<string, Dictionary<string, int>>();
			return;
		}

		try
		{
			var json = File.ReadAllText(saveFile);
			SavedOceanBreedCounts = JsonUtility.FromJson<Dictionary<string, Dictionary<string, int>>>(json);
		}
		catch (Exception e)
		{
			_hasLoadError = true;
			ErrorMessage.AddDebug(LoadError);

			do
			{
				Main.Logger.LogError(e.Message);
				Main.Logger.LogError(e.StackTrace);
				e = e.InnerException;
			}
			while (e != null);
		}
	}

	private static IEnumerator SpawnCreature(WaterPark waterPark, TechType parkCreatureTechType, ItemsContainer container)
	{
		var spawnPoint = waterPark.transform.position + (UnityEngine.Random.insideUnitSphere * 50);
		if (container is null)
		{
			if (waterPark.transform.position.y > 0) yield break;
			var @base = waterPark.hostBase;
			while (Vector3.Distance(@base.GetClosestPoint(spawnPoint), spawnPoint) < 25 || spawnPoint.y >= 0)
			{
				yield return null;
				spawnPoint = @base.GetClosestPoint(spawnPoint) + (UnityEngine.Random.insideUnitSphere * 50);
			}
		}

		var task = CraftData.GetPrefabForTechTypeAsync(parkCreatureTechType, false);
		yield return task;

		var prefab = task.GetResult();
		if (prefab == null) yield break;

		prefab.SetActive(false);
		var gameObject = GameObject.Instantiate(prefab);

		if (container is not null)
		{
			var pickupable = gameObject.EnsureComponent<Pickupable>();
			pickupable.Pickup(false);
			gameObject.SetActive(false);
			container.AddItem(pickupable);
			yield break;
		}

		Creature creature = gameObject.GetComponent<Creature>();
		if (creature != null)
		{
			creature.friendlyToPlayer = true;
		}

		gameObject.transform.SetPositionAndRotation(spawnPoint, Quaternion.identity);
		gameObject.SetActive(true);
	}
}
