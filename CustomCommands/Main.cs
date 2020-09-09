using CustomCommands.MonoBehaviours;
using QModManager.API.ModLoading;
using System.Collections.Generic;
#if SN1
using QModManager.Utility;
using System;
#endif
using HarmonyLib;
using System.Reflection;

namespace CustomCommands
{
	[QModCore]
    public static class Main
    {
#if SN1
		internal static Dictionary<TechType, WaterParkCreatureParameters> CreatureParameters = new Dictionary<TechType, WaterParkCreatureParameters>()
		{
			{ TechType.ReaperLeviathan, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, true) },
			{ TechType.SeaDragon, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, true) },
			{ TechType.GhostLeviathan, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, true) },
			{ TechType.SeaEmperorJuvenile, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, true) },
			{ TechType.SeaEmperorBaby, new WaterParkCreatureParameters(0.1f, 0.3f, 1f, 3f, true) },
			{ TechType.GhostLeviathanJuvenile, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, true) },
			{ TechType.Warper, new WaterParkCreatureParameters(0.05f, 0.2f, 1f, 3f, true) },
		};
#elif BZ
		internal static Dictionary<TechType, WaterParkCreatureData> CreatureParameters = new Dictionary<TechType, WaterParkCreatureData>()
		{
			{ TechType.Chelicerate, new WaterParkCreatureData()
			{ initialSize = 0.01f, maxSize = 0.05f, outsideSize = 1f, growingPeriodInDays = 3f, isPickupableOutside = true,
				adultPrefab = CraftData.GetPrefabForTechType(TechType.Chelicerate), canBreed = true, eggOrChildPrefab = CraftData.GetPrefabForTechType(TechType.Chelicerate)  } },
			{ TechType.ShadowLeviathan, new WaterParkCreatureData()
			{ initialSize = 0.01f, maxSize = 0.05f, outsideSize = 1f, growingPeriodInDays =3f, isPickupableOutside = true,
				adultPrefab = CraftData.GetPrefabForTechType(TechType.ShadowLeviathan), canBreed = true, eggOrChildPrefab = CraftData.GetPrefabForTechType(TechType.ShadowLeviathan) } },
			{ TechType.IceWorm, new WaterParkCreatureData()
			{ initialSize = 0.01f, maxSize = 0.05f, outsideSize = 1f, growingPeriodInDays =3f, isPickupableOutside = true,
				adultPrefab = CraftData.GetPrefabForTechType(TechType.IceWorm), canBreed = true, eggOrChildPrefab = CraftData.GetPrefabForTechType(TechType.IceWorm) } },
			{ TechType.SeaEmperorJuvenile, new WaterParkCreatureData()
			{ initialSize = 0.01f, maxSize = 0.05f, outsideSize = 1f, growingPeriodInDays =3f, isPickupableOutside = true,
				adultPrefab = CraftData.GetPrefabForTechType(TechType.SeaEmperorJuvenile), canBreed = true, eggOrChildPrefab = CraftData.GetPrefabForTechType(TechType.SeaEmperorJuvenile) } },
			{ TechType.Warper, new WaterParkCreatureData()
			{ initialSize = 0.05f, maxSize = 0.2f, outsideSize = 1f, growingPeriodInDays =3f, isPickupableOutside = true,
				adultPrefab = CraftData.GetPrefabForTechType(TechType.Warper), canBreed = true, eggOrChildPrefab = CraftData.GetPrefabForTechType(TechType.Warper) } },
		};
#endif

		[QModPatch]
        public static void Load()
		{
#if SN1
			foreach (KeyValuePair<TechType, WaterParkCreatureParameters> pair in CreatureParameters)
			{
				try
				{
					WaterParkCreature.waterParkCreatureParameters[pair.Key] = pair.Value;
				}
				catch (Exception e)
				{
					Logger.Log(Logger.Level.Debug, $"Failed to add {pair.Key} into the Aquarium Database.", e);
				}
			}
#endif
			Assembly assembly = Assembly.GetExecutingAssembly();
			new Harmony($"Coticvo_{assembly.GetName().Name}").PatchAll(assembly);

			Placeholder.Awake();
		}
    }
}