using HarmonyLib;
using MAC.ScubaManifold;
using NitrogenMod.Items;
using NitrogenMod.NMBehaviours;
using UnityEngine;

namespace SpecialtyManifold.Patches
{
    [HarmonyPatch(typeof(SpecialtyTanks), nameof(SpecialtyTanks.Update))]
    public class SpecialtyTanks_Update_Patch
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
                    float photosynthesisDepthCalc = (currentLight > 0.9f ? 0.9f : currentLight) * Time.deltaTime * (Main.config.multipleTanks ? photosynthesisTanks : 1) * (200f - playerDepth > 0f ? ((200 - playerDepth) / 200f) : 0);
                    Player.main.oxygenMgr.AddOxygen(photosynthesisDepthCalc);
                }

                if (chemosynthesisTanks > 0)
                {
                    float waterTemp = WaterTemperatureSimulation.main.GetTemperature(Player.main.transform.position);
                    float chemosynthesisTempCalc = (waterTemp > 30f ? waterTemp : 0) * Time.deltaTime * 0.01f * (Main.config.multipleTanks ? chemosynthesisTanks : 1);
                    Player.main.oxygenMgr.AddOxygen(chemosynthesisTempCalc);
                }
            }
        }
    }
}