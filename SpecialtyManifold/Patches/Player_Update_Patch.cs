namespace SpecialtyManifold.Patches
{
    using HarmonyLib;
    using SMLHelper.Handlers;
    using UnityEngine;

    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public class Player_Update_Patch
    {
        public static TechType scubaManifold = TechType.None;
        public static TechType photosynthesisSmall = TechType.None;
        public static TechType photosynthesisTank = TechType.None;
        public static TechType chemosynthesisTank = TechType.None;
        public static bool modCheck = true;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if(modCheck)
            {
                TechTypeHandler.TryGetModdedTechType("ScubaManifold", out scubaManifold);
                TechTypeHandler.TryGetModdedTechType("photosynthesissmalltank", out photosynthesisSmall);
                TechTypeHandler.TryGetModdedTechType("photosynthesistank", out photosynthesisTank);
                TechTypeHandler.TryGetModdedTechType("chemosynthesistank", out chemosynthesisTank);
                modCheck = false;
            }

            if(scubaManifold != TechType.None && photosynthesisSmall != TechType.None && photosynthesisTank != TechType.None && chemosynthesisTank != TechType.None)
            {
                var tankSlot = Inventory.main.equipment.GetTechTypeInSlot("Tank");
                if(GameModeUtils.RequiresOxygen() && Player.main.IsSwimming() && tankSlot == scubaManifold)
                {
                    var photosynthesisTanks = Inventory.main.container.GetCount(photosynthesisSmall) + Inventory.main.container.GetCount(photosynthesisTank);
                    var chemosynthesisTanks = Inventory.main.container.GetCount(chemosynthesisTank);

                    if(photosynthesisTanks > 0)
                    {
                        var playerDepth = Ocean.GetDepthOf(Player.main.gameObject);
                        var currentLight = DayNightCycle.main.GetLocalLightScalar();
                        var photosynthesisDepthCalc = (currentLight > 0.9f ? 0.9f : currentLight) * Time.deltaTime * (Main.SMLConfig.multipleTanks ? photosynthesisTanks : 1) * (200f - playerDepth > 0f ? ((200 - playerDepth) / 200f) : 0);
                        Player.main.oxygenMgr.AddOxygen(photosynthesisDepthCalc);
                    }

                    if(chemosynthesisTanks > 0)
                    {
                        var waterTemp = WaterTemperatureSimulation.main.GetTemperature(Player.main.transform.position);
                        var chemosynthesisTempCalc = (waterTemp > 30f ? waterTemp : 0) * Time.deltaTime * 0.01f * (Main.SMLConfig.multipleTanks ? chemosynthesisTanks : 1);
                        Player.main.oxygenMgr.AddOxygen(chemosynthesisTempCalc);
                    }
                }
            }
        }
    }
}