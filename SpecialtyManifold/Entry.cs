using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MAC.ScubaManifold;
using NitrogenMod.Items;
using NitrogenMod.NMBehaviours;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using UnityEngine;

namespace SpecialtyManifold
{
    public static class Config
    {
        public static bool multipleTanks;

        public static void Load()
        {
            multipleTanks = PlayerPrefs.GetInt("multipleTanks", 1) > 0;
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

    [QModCore]
    public class Entry
    {
        [QModPatch]
        public static void Patch()
        {
            try
            {
                Config.Load();
                new Harmony("MrPurple6411_SpecialtyManifold").PatchAll(Assembly.GetExecutingAssembly());

                KnownTechHandler.SetAnalysisTechEntry(TechType.Workbench, new List<TechType>() { O2TanksCore.PhotosynthesisSmallID, O2TanksCore.PhotosynthesisTankID, O2TanksCore.ChemosynthesisTankID });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Specialty Manifold")
        {
            ToggleChanged += Options_multipleTanksChanged;
        }

        public override void BuildModOptions()
        {
            AddToggleOption("multipleTanks", "Effects of multiple tanks?", Config.multipleTanks);
        }

        public void Options_multipleTanksChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "multipleTanks")
            {
                return;
            }

            Config.multipleTanks = e.Value;
            PlayerPrefs.SetInt("multipleTanks", e.Value ? 1 : 0);
        }
    }

    [HarmonyPatch(typeof(SpecialtyTanks))]
    [HarmonyPatch(nameof(SpecialtyTanks.Update))]
    internal class SpecialtyTanks_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            TechType tankSlot = Inventory.main.equipment.GetTechTypeInSlot("Tank");
            if (GameModeUtils.RequiresOxygen() && Player.main.IsSwimming() && tankSlot == ScubaManifold.techType)
            {
                int photosynthesisTanks = Inventory.main.container.GetCount(O2TanksCore.PhotosynthesisSmallID) + Inventory.main.container.GetCount(O2TanksCore.PhotosynthesisTankID);
                int chemosynthesisTanks = Inventory.main.container.GetCount(O2TanksCore.ChemosynthesisTankID);

                if (photosynthesisTanks > 0)
                {
                    float playerDepth = Ocean.main.GetDepthOf(Player.main.gameObject);
                    float currentLight = DayNightCycle.main.GetLocalLightScalar();
                    float photosynthesisDepthCalc = (currentLight > 0.9f ? 0.9f : currentLight) * Time.deltaTime * (Config.multipleTanks ? photosynthesisTanks : 1) * (200f - playerDepth > 0f ? ((200 - playerDepth) / 200f) : 0);
                    Player.main.oxygenMgr.AddOxygen(photosynthesisDepthCalc);
                }

                if (chemosynthesisTanks > 0)
                {
                    float waterTemp = WaterTemperatureSimulation.main.GetTemperature(Player.main.transform.position);
                    float chemosynthesisTempCalc = (waterTemp > 30f ? waterTemp : 0) * Time.deltaTime * 0.01f * (Config.multipleTanks ? chemosynthesisTanks : 1);
                    Player.main.oxygenMgr.AddOxygen(chemosynthesisTempCalc);
                }
            }
        }
    }
}