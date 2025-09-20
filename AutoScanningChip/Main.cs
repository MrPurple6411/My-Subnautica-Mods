namespace AutoScanningChip;

using System.Collections;
using System.Collections.Generic;
using BepInEx;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;
using BepInEx.Logging;

using AutoScanningChip.Configuration;
// no patches needed in timer-based mode

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main : BaseUnityPlugin
{
	public static TechType ChipTechType { get; private set; }
	internal static ManualLogSource Log;
	internal static Main Instance { get; private set; }

	// Pulse scheduler state
	private float _lastPulseTime = -9999f;
	private readonly HashSet<string> _activeScanKeys = new HashSet<string>();

	private void Awake()
	{
		Instance = this;

		// Prefab metadata for the new chip
		var info = PrefabInfo.WithTechType(
			classId: "AutoScanningChip",
			displayName: "Auto-Scanning Chip",
			description: "Automatically scans scannable targets when you bump into them.",
			unlockAtStart: true);

		// Remember our TechType for equipped count checks
		ChipTechType = info.TechType;

		// Use the Map Room HUD Chip icon as a placeholder
		info.WithIcon(SpriteManager.Get(TechType.MapRoomHUDChip));

		// Create a prefab by cloning the MapRoomHUDChip so it looks like a standard chip
		var prefab = new CustomPrefab(info);

		var clone = new CloneTemplate(info, TechType.MapRoomHUDChip);

		prefab.SetGameObject(clone);

		// Mark as an equipable chip (attach the gadget to the prefab so it builds during registration)
		prefab.SetEquipment(EquipmentType.Chip)
			.WithQuickSlotType(QuickSlotType.None);

		// Basic, sensible recipe (tweak as desired)
		prefab.SetRecipe(new RecipeData(
			new Ingredient(TechType.ComputerChip, 1),
			new Ingredient(TechType.CopperWire, 1)
		))
		.WithFabricatorType(CraftTree.Type.Fabricator)
		.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorEquipment);

		// Place under PDA Blueprints: Personal -> Equipment
		prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);

		// Register item
		prefab.Register();

		// Ensure Nautilus options are initialized
		var cfg = ASCSettings.Instance;
		Logger.LogInfo($"Auto-Scanning Chip registered. Config: speedx={cfg.scanSpeedMultiplier}, baseR={cfg.baseRadius}, perChipScale={cfg.perChipScale}, pulse={cfg.pulseIntervalSeconds}s");

		// Expose logger for diagnostics
		Log = Logger;
	}

	private void Update()
	{
		// 1) Player exists
		var player = Player.main;
		if (player == null) return;

		// 2) Inventory exists
		var inv = Inventory.main;
		if (inv == null) return;

		// 3) Check equipped count
		int count = inv.equipment?.GetCount(ChipTechType) ?? 0;
		if (count <= 0)
		{
			// Not equipped: clear any in-flight scans and do nothing
			if (_activeScanKeys.Count > 0)
				_activeScanKeys.Clear();
			return;
		}

		// 4) Pulse every configured interval
		float interval = Mathf.Max(0.5f, ASCSettings.Instance.pulseIntervalSeconds);
		if (Time.time - _lastPulseTime >= interval)
		{
			PulseScanImmediate();
			_lastPulseTime = Time.time;
		}
	}

	public void ScheduleImmediatePulse()
	{
		// Will be executed on next Update()
		_lastPulseTime = -9999f;
	}

	// Manual overlap pass to find scannables around the player
	public void PulseScanImmediate()
	{
		try
		{
			var player = Player.main;
			if (player == null) return;

			var pos = player.transform.position;
			int count = Inventory.main?.equipment?.GetCount(ChipTechType) ?? 0;
			if (count <= 0) return;

			float baseR = Mathf.Max(0.1f, ASCSettings.Instance.baseRadius);
			float scale = Mathf.Max(1f, count * Mathf.Max(0.01f, ASCSettings.Instance.perChipScale));
			float worldRadius = baseR * scale;

			var results = Physics.OverlapSphere(pos, worldRadius, ~0, QueryTriggerInteraction.Collide);
			if (results == null || results.Length == 0) return;

			foreach (var col in results)
			{
				if (col == null) continue;
				TryBeginScan(col.gameObject);
			}
		}
		catch (System.Exception)
		{
			// Keep silent; this is a best-effort pulse
		}
	}

	private void TryBeginScan(GameObject go)
	{
		if (go == null) return;

		var dataBox = go.GetComponentInParent<BlueprintHandTarget>();
		if (dataBox != null)
		{
			if (dataBox.used)
				return;

			dataBox.used = dataBox.TryToAddToKnownTech();

			if (!dataBox.used)
				return;

			if (!string.IsNullOrEmpty(dataBox.onUseGoal.key))
			{
				dataBox.onUseGoal.Trigger();
			}
			
			dataBox.OnTargetUsed();
			return;
		}

		PDAScanner.ScanTarget scanTarget = default(PDAScanner.ScanTarget);
		scanTarget.Invalidate();
		scanTarget.Initialize(go);

		PDAScanner.Result result = PDAScanner.CanScan(scanTarget);
		if (result != PDAScanner.Result.Scan
#if SUBNAUTICA
			|| scanTarget.isPlayer
#endif
		)
		{
			return;
		}

		string key = GetScanKey(go, scanTarget);
		if (key == null || _activeScanKeys.Contains(key))
			return;

		_activeScanKeys.Add(key);
		StartCoroutine(Scan(scanTarget, key));
	}

	private static string GetScanKey(GameObject go, PDAScanner.ScanTarget scanTarget)
	{
		if (scanTarget.hasUID && !string.IsNullOrEmpty(scanTarget.uid))
			return scanTarget.uid;
		return go ? $"inst:{go.GetInstanceID()}" : null;
	}

	private IEnumerator Scan(PDAScanner.ScanTarget scanTarget, string key)
	{
		PDAScanner.EntryData entryData = PDAScanner.GetEntryData(scanTarget.techType);
		float num = 2f;
		if (entryData != null)
		{
			num = entryData.scanTime;
		}
		if (NoCostConsoleCommand.main.fastScanCheat)
		{
			num = 0.01f;
		}
		// Apply speed multiplier (1.0=stock, >1 faster, <1 slower)
		num /= Mathf.Max(0.01f, ASCSettings.Instance.scanSpeedMultiplier);

		if (scanTarget.hasUID && PDAScanner.fragments.ContainsKey(scanTarget.uid))
		{
			scanTarget.progress = PDAScanner.fragments[scanTarget.uid];
		}

		bool isTechTypeComplete = PDAScanner.complete.Contains(scanTarget.techType);
		if (isTechTypeComplete)
		{
			scanTarget.progress = 1f;
		}

		float nextLog = 0.1f;
		while (scanTarget.progress < 1f)
		{
			// Abort if this scan was cancelled externally (e.g., chip unequipped)
			if (!_activeScanKeys.Contains(key))
				yield break;

			yield return null;
			scanTarget.progress += Time.deltaTime / num;

			if (scanTarget.hasUID)
			{
				PDAScanner.cachedProgress[scanTarget.uid] = scanTarget.progress;
			}
			if (scanTarget.progress >= nextLog)
			{
				nextLog += 0.1f;
			}
		}

		HandleScanCompletion(scanTarget, entryData, isTechTypeComplete);
		_activeScanKeys.Remove(key);
	}

	private static void HandleScanCompletion(PDAScanner.ScanTarget scanTarget, PDAScanner.EntryData entryData, bool isTechTypeComplete)
	{
		bool unlockBlueprint = false;
		bool unlockEncyclopedia = false;

		scanTarget.progress = 0f;
		if (scanTarget.hasUID && PDAScanner.fragments != null && !PDAScanner.fragments.ContainsKey(scanTarget.uid))
		{
			PDAScanner.fragments.Add(scanTarget.uid, 1f);
		}
		if (entryData != null)
		{
			unlockEncyclopedia = true;
			if (!isTechTypeComplete)
			{
				PDAScanner.Entry entry;
				if (!PDAScanner.GetPartialEntryByKey(scanTarget.techType, out entry))
				{
					entry = PDAScanner.Add(scanTarget.techType, 0);
				}
				if (entry != null)
				{
					entry.unlocked++;
					if (entry.unlocked >= entryData.totalFragments)
					{
						unlockBlueprint = true;
						PDAScanner.partial.Remove(entry);
						PDAScanner.complete.Add(entry.techType);
						PDAScanner.NotifyRemove(entry);
					}
					else
					{
						int totalFragments = entryData.totalFragments;
						if (totalFragments > 1)
						{
							float pct = Mathf.RoundToInt((float) entry.unlocked / (float) totalFragments * 100f);
							ErrorMessage.AddError(Language.main.GetFormat<string, float, int, int>("ScannerInstanceScanned", Language.main.Get(scanTarget.techType.AsString(false)), pct, entry.unlocked, totalFragments));
						}
						PDAScanner.NotifyProgress(entry);
					}
				}
			}
		}
		scanTarget.gameObject?.SendMessage("OnScanned", entryData, SendMessageOptions.DontRequireReceiver);
		if (unlockBlueprint || unlockEncyclopedia)
		{
			PDAScanner.Unlock(entryData, unlockBlueprint, unlockEncyclopedia, true);

			TechType harvestedType = TechData.GetHarvestOutput(scanTarget.techType);
			if (harvestedType != TechType.None)
			{
				var entryData2 = PDAScanner.GetEntryData(harvestedType);
				if (entryData2 != null)
				{
					PDAScanner.Unlock(entryData2, unlockBlueprint, unlockEncyclopedia, true);
				}
			}

			BreakableResource component = scanTarget.gameObject?.GetComponent<BreakableResource>();
			if (component != null)
			{
				var entryData3 = PDAScanner.GetEntryData(component.defaultPrefabTechType);
				if (entryData3 != null)
				{
					PDAScanner.Unlock(entryData3, unlockBlueprint, unlockEncyclopedia, true);
				}

				var list = component.prefabList;
				if (list != null)
				{
					foreach (BreakableResource.RandomPrefab resource in list)
					{
						if (resource == null || resource.prefabTechType == TechType.None) continue;

						var entryData4 = PDAScanner.GetEntryData(resource.prefabTechType);
						if (entryData4 != null)
						{
							PDAScanner.Unlock(entryData4, unlockBlueprint, unlockEncyclopedia, true);
						}
					}
				}
			}
		}
#if SUBNAUTICA
		var infectedMixin = scanTarget.gameObject?.GetComponent<InfectedMixin>();
		if (infectedMixin?.IsInfected() ?? false)
		{
			PDAEncyclopedia.Add("Infection", true);
			if (infectedMixin.IsHealedByPeeper())
			{
				PDAEncyclopedia.Add("CuredCreature", true);
			}
		}
		if (scanTarget.techType == TechType.Peeper && scanTarget.gameObject != null)
		{
			Peeper peeper = scanTarget.gameObject?.GetComponent<Peeper>();
			if (peeper != null && peeper.isHero)
			{
				PDAEncyclopedia.Add("HeroPeeper", true);
			}
			if (peeper != null && peeper.isInPrisonAquarium)
			{
				PDAEncyclopedia.Add("AquariumPeeper", true);
			}
		}
#endif

		if (entryData != null && entryData.destroyAfterScan && scanTarget.gameObject != null)
		{
			UnityEngine.Object.Destroy(scanTarget.gameObject);
			scanTarget.Invalidate();
			if (scanTarget.hasUID && PDAScanner.fragments != null)
			{
				PDAScanner.fragments.Remove(scanTarget.uid);
			}
		}
	}
}

// no proxy or player-attached components needed in timer-based mode

