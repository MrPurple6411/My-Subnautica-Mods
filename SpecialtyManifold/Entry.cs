using Harmony;
using MAC.ScubaManifold;
using NitrogenMod.Items;
using NitrogenMod.NMBehaviours;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SpecialtyManifold
{
    [QModCore]
    public class Entry
    {
        [QModPatch]
        public static void Patch()
        {
            try
            {
                HarmonyInstance.Create("MrPurple6411.SpecialtyManifold").PatchAll(Assembly.GetExecutingAssembly());
				Config.Load();
				OptionsPanelHandler.RegisterModOptions(new Options());
			}
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }


    public static class Config
    {
        public static bool multipleTanks;

        public static void Load()
        {
            multipleTanks = PlayerPrefs.GetInt("multipleTanks", 1)>0;
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Specialty Manifold")
        {
            ToggleChanged += Options_multipleTanksChanged;
        }

        public void Options_multipleTanksChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "multipleTanks") return;
            Config.multipleTanks = e.Value;
            PlayerPrefs.SetInt("multipleTanks", e.Value? 1:0);
        }

        public override void BuildModOptions()
        {
            AddToggleOption("multipleTanks", "Effects of multiple tanks?", Config.multipleTanks);
        }
    }

    [HarmonyPatch(typeof(SpecialtyTanks))]
    [HarmonyPatch(nameof(SpecialtyTanks.Update))]
    class SpecialtyTanks_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
		{
			if (GameModeUtils.RequiresOxygen())
			{
				TechType tankSlot = Inventory.main.equipment.GetTechTypeInSlot("Tank");
				int photosynthesisTanks = Inventory.main.container.GetCount(O2TanksCore.PhotosynthesisSmallID) + Inventory.main.container.GetCount(O2TanksCore.PhotosynthesisTankID);
				int chemosynthesisTanks = Inventory.main.container.GetCount(O2TanksCore.ChemosynthesisTankID);
				float playerDepth = Ocean.main.GetDepthOf(Player.main.gameObject);
				float waterTemp = WaterTemperatureSimulation.main.GetTemperature(Player.main.transform.position);
				float currentLight = DayNightCycle.main.GetLocalLightScalar();
                float chemosynthesisTanksadded = 0;
                float photosynthesisTanksadded = 0;

                float photosynthesisDepthCalc = ((currentLight > 0.9f ? 0.9f : currentLight) * Time.deltaTime * (Config.multipleTanks ? photosynthesisTanks : 1)) * ((200f - playerDepth > 0f ? ((200 - playerDepth) / 200f) : 0));
                float chemosynthesisTempCalc = ((waterTemp > 30f ? waterTemp : 0) * Time.deltaTime * 0.01f) * (Config.multipleTanks ? chemosynthesisTanks : 1);

                if (tankSlot == ScubaManifold.techType)
				{
					if (photosynthesisTanks > 0)
                        photosynthesisTanksadded = Player.main.oxygenMgr.AddOxygen(photosynthesisDepthCalc);

					if (chemosynthesisTanks > 0)
                        chemosynthesisTanksadded = Player.main.oxygenMgr.AddOxygen(chemosynthesisTempCalc);
                }
#if DEBUG
                ErrorMessage.AddMessage($"photosynthesisDepthCalc: {Math.Round(photosynthesisDepthCalc, 2)}");
                ErrorMessage.AddMessage($"chemosynthesisTempCalc: {Math.Round(chemosynthesisTempCalc, 2)}");
#endif
            }
        }
    }
}
