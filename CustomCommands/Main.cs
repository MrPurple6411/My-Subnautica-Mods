namespace CustomCommands;

using MonoBehaviours;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Nautilus.Extensions;
using UWE;
using BepInEx.Logging;
using Nautilus.Handlers;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main : BaseUnityPlugin
{
	private static GameObject _dummyObject;
	private static new ManualLogSource Logger;

	/// <summary>
	/// WaterParkCreatureParameters(float initialSize, float maxSize, float outsideSize, Vector2int itemSize, float growingPeriodInDays, bool isPickupableOutside)
	/// </summary>
	private static readonly Dictionary<TechType, WaterParkCreatureParameters> _creatureParameters = new()
	{
#if SUBNAUTICA
			{ TechType.ReaperLeviathan, new WaterParkCreatureParameters(0.01f, 0.05f, 1, new Vector2int(3, 3), 3, false) },
			{ TechType.SeaDragon, new WaterParkCreatureParameters(0.01f, 0.025f, 1, new Vector2int(3, 3), 3, false) },
			{ TechType.GhostLeviathan, new WaterParkCreatureParameters(0.01f, 0.025f, 1, new Vector2int(3, 3), 3, false) },
			{ TechType.GhostLeviathanJuvenile, new WaterParkCreatureParameters(0.01f, 0.025f, 1, new Vector2int(3, 3), 3, false) },
			{ TechType.SeaEmperorJuvenile, new WaterParkCreatureParameters(0.01f, 0.025f, 1, new Vector2int(3, 3), 3, false) },
			{ TechType.SeaEmperorBaby, new WaterParkCreatureParameters(0.01f, 0.01f, 1, new Vector2int(2, 2), 3, false) },
			{ TechType.Warper, new WaterParkCreatureParameters(0.1f, 0.25f, 1, new Vector2int(2, 2),  3, false) },
#else

#endif
	};

	public void Awake()
	{
		Logger = base.Logger;
		Initialize();
	}

	internal static void Initialize()
	{
		if (_dummyObject != null)
			Object.DestroyImmediate(_dummyObject);
		_dummyObject = new GameObject("DummyObject");
		_dummyObject.AddComponent<SceneCleanerPreserve>();
		Object.DontDestroyOnLoad(_dummyObject);
		Commands commands = _dummyObject.AddComponent<Commands>();

		commands.StartCoroutine(ProcessCreatureParameters());

	}

	private static IEnumerator ProcessCreatureParameters()
	{
		foreach (KeyValuePair<TechType, WaterParkCreatureParameters> creatureParameter in _creatureParameters)
		{
			TechType techType = creatureParameter.Key;
			WaterParkCreatureParameters parameters = creatureParameter.Value;

			var task = CraftData.GetPrefabForTechTypeAsync(techType, false);
			yield return task;
			var prefab = task.GetResult();

			if (prefab is null)
			{
				ErrorMessage.AddDebug("Unable to find prefab for " + techType.AsString());
				continue;
			}

			Logger.LogDebug("Processing " + Language.main.Get(techType));

			if (!(prefab.GetComponent<WaterParkCreature>() is WaterParkCreature parkCreature))
				parkCreature = prefab.AddComponent<WaterParkCreature>();

			if (prefab.TryGetComponent(out SplineFollowing spline))
				spline.targetRange = 1f;

			CraftDataHandler.SetItemSize(techType, parameters.itemSize);
			if (parkCreature.data == null)
			{
				var data = ScriptableObject.CreateInstance<WaterParkCreatureData>();

				data.initialSize = parameters.initialSize;
				data.maxSize = parameters.maxSize;
				data.outsideSize = parameters.outsideSize;
				data.daysToGrow = parameters.daysToGrow;
				data.isPickupableOutside = parameters.isPickupableOutside;
				data.canBreed = true;

				var classid = CraftData.GetClassIdForTechType(techType);
				string fileName = PrefabDatabase.TryGetPrefabFilename(classid, out var filename) ? filename : "";

				if (!string.IsNullOrWhiteSpace(fileName))
				{
					data.adultPrefab = new(filename);
					data.adultPrefab.ForceValid();
					data.eggOrChildPrefab = new(filename);
					data.eggOrChildPrefab.ForceValid();
				}
				else
				{
					ErrorMessage.AddDebug("Unable to find prefab file name for " + techType.AsString());
				}

				parkCreature.data = data;
			}

		}
	}

	private struct WaterParkCreatureParameters
	{
		public readonly float initialSize;

		public readonly float maxSize;

		public readonly float outsideSize;

		public readonly Vector2int itemSize;

		public readonly float daysToGrow;

		public readonly bool isPickupableOutside;

		public WaterParkCreatureParameters() { }

		public WaterParkCreatureParameters(float initialSize, float maxSize, float outsideSize, Vector2int itemSize, float daysToGrow, bool isPickupableOutside)
		{
			this.initialSize = initialSize;
			this.maxSize = maxSize;
			this.outsideSize = outsideSize;
			this.itemSize = itemSize;
			this.daysToGrow = daysToGrow;
			this.isPickupableOutside = isPickupableOutside;
		}
	}
}