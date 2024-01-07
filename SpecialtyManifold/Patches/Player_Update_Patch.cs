namespace SpecialtyManifold.Patches;

using HarmonyLib;
using Nautilus.Handlers;
using SpecialtyManifold.Configuration;
using UnityEngine;

[HarmonyPatch(typeof(Player), nameof(Player.Update))]
public class Player_Update_Patch
{
	public static Config Config { get; internal set; }
	public static TechType scubaManifold = TechType.None;
	public static TechType photosynthesisSmall = TechType.None;
	public static TechType photosynthesisTank = TechType.None;
	public static TechType chemosynthesisTank = TechType.None;
	public static bool modsCheck = true;
	public static bool modsFound = false;

	[HarmonyPostfix]
	public static void Postfix()
	{
		if (modsCheck)
		{
			modsCheck = false;
			modsFound = EnumHandler.TryGetValue("ScubaManifold", out scubaManifold) &&
			EnumHandler.TryGetValue("photosynthesissmalltank", out photosynthesisSmall) &&
			EnumHandler.TryGetValue("photosynthesistank", out photosynthesisTank) &&
			EnumHandler.TryGetValue("chemosynthesistank", out chemosynthesisTank);

			if (modsFound)
				Config = OptionsPanelHandler.RegisterModOptions<Config>();
		}

		if (!modsFound)
			return;

		var tankSlot = Inventory.main.equipment.GetTechTypeInSlot("Tank");
		if (
#if SUBNAUTICA
			!GameModeUtils.RequiresOxygen() || 
#endif
			!Player.main.IsSwimming() || tankSlot != scubaManifold)
			return;

		var photosynthesisTanks = Inventory.main.container.GetCount(photosynthesisSmall) + Inventory.main.container.GetCount(photosynthesisTank);

		if (photosynthesisTanks > 0)
		{
			var playerDepth = Ocean.GetDepthOf(Player.main.gameObject);
			var currentLight = DayNightCycle.main.GetLocalLightScalar();
			var photosynthesisDepthCalc = (currentLight > 0.9f ? 0.9f : currentLight) * Time.deltaTime * (Config.multipleTanks ? photosynthesisTanks : 1) * (200f - playerDepth > 0f ? ((200 - playerDepth) / 200f) : 0);
			Player.main.oxygenMgr.AddOxygen(photosynthesisDepthCalc);
		}

		var chemosynthesisTanks = Inventory.main.container.GetCount(chemosynthesisTank);
		if (chemosynthesisTanks > 0)
		{
			var waterTemp = WaterTemperatureSimulation.main.GetTemperature(Player.main.transform.position);
			var chemosynthesisTempCalc = (waterTemp > 30f ? waterTemp : 0) * Time.deltaTime * 0.01f * (Config.multipleTanks ? chemosynthesisTanks : 1);
			Player.main.oxygenMgr.AddOxygen(chemosynthesisTempCalc);
		}
	}
}