using Harmony;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UWE;

namespace BetterACU
{
    [QModCore]
    public class Entry
    {
        [QModPatch]
        public static void Patch()
        {
            HarmonyInstance.Create("MrPurple6411_BetterACU").PatchAll(Assembly.GetExecutingAssembly());
            Config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

    public static class Config
    {
        public static int WaterParkSize;
        public static int GrowthMultiplier;
        public static int BreedSpeed;

        public static void Load()
        {
            WaterParkSize = PlayerPrefs.GetInt("WaterParkSize", 10);
            GrowthMultiplier = PlayerPrefs.GetInt("GrowthMultiplier", 1);
            BreedSpeed = PlayerPrefs.GetInt("BreedSpeed", 1);
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Better ACU")
        {
            SliderChanged += BetterACU_SliderChanged;
        }

        public void BetterACU_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            List<string> ids = new List<string>() { "WaterParkSize", "GrowthMultiplier", "BreedSpeed" };

            if (!ids.Contains(e.Id)) return;
            switch (e.Id)
            {
                case "WaterParkSize":
                    Config.WaterParkSize = (int)e.Value;
                    PlayerPrefs.SetInt("WaterParkSize", (int)e.Value);
                    break;

                case "GrowthMultiplier":
                    Config.GrowthMultiplier = (int)e.Value;
                    PlayerPrefs.SetInt("GrowthMultiplier", (int)e.Value);
                    break;

                case "BreedSpeed":
                    Config.BreedSpeed = (int)e.Value;
                    PlayerPrefs.SetInt("BreedSpeed", (int)e.Value);
                    break;

                default:
                    break;
            }

        }

        public override void BuildModOptions()
        {
            AddSliderOption("WaterParkSize", "Alien Containment Limit", 10, 100, Config.WaterParkSize);
            AddSliderOption("GrowthMultiplier", "Plant Growth Speed", 1, 100, Config.GrowthMultiplier);
            AddSliderOption("BreedSpeed", "Fish Breeding Speed", 1, 100, Config.BreedSpeed);
        }
    }
    [HarmonyPatch(typeof(WaterPark))]
    [HarmonyPatch(nameof(WaterPark.Update))]
    internal class WaterPark_Update_Postfix
    {
        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance)
        {
            if (__instance.wpPieceCapacity != Config.WaterParkSize)
            {
                __instance.wpPieceCapacity = Config.WaterParkSize;
            }

            foreach (Planter.PlantSlot plantSlot in __instance.planter.smallPlantSlots)
            {
                Plantable plantable = plantSlot.plantable;
                if (plantable != null)
                {
                    GrowingPlant growingPlant = plantSlot.plantable.growingPlant;
                    if (growingPlant.growthDuration != (1 / Config.GrowthMultiplier))
                    {
                        growingPlant.growthDuration = 1 / Config.GrowthMultiplier;
                    }
                }
            }

            foreach (Planter.PlantSlot plantSlot in __instance.planter.bigPlantSlots)
            {
                Plantable plantable = plantSlot.plantable;
                if (plantable != null)
                {
                    GrowingPlant growingPlant = plantSlot.plantable.growingPlant;
                    if (growingPlant.growthDuration != (1 / Config.GrowthMultiplier))
                    {
                        growingPlant.growthDuration = 1 / Config.GrowthMultiplier;
                    }
                }
            }

            foreach (WaterParkItem waterParkItem in __instance.items)
            {
                WaterParkCreature parkCreature = waterParkItem as WaterParkCreature;
                if (parkCreature.breedInterval != 1 / Config.BreedSpeed)
                    parkCreature.breedInterval = 1 / Config.BreedSpeed;

                if (parkCreature.canBreed)
                    __instance.TryBreed(parkCreature);
            }
        }

        [HarmonyPatch(typeof(WaterPark))]
        [HarmonyPatch(nameof(WaterPark.TryBreed))]
        internal class WaterPark_TryBreed_Prefix
        {
            [HarmonyPostfix]
            public static void Postfix(WaterPark __instance, WaterParkCreature creature)
            {

                if (!creature.canBreed) return;

                List<BaseBioReactor> baseBioReactors = new List<BaseBioReactor>();
                baseBioReactors = __instance.gameObject.GetComponentInParent<SubRoot>().gameObject.GetComponentsInChildren<BaseBioReactor>().ToList();


                WaterParkCreature parkCreature = null;
                foreach (WaterParkItem waterParkItem in __instance.items)
                {
                    parkCreature = waterParkItem as WaterParkCreature;
                    if (waterParkItem != creature && parkCreature != null && parkCreature.GetCanBreed() && CraftData.GetTechType(parkCreature.gameObject) == CraftData.GetTechType(creature.gameObject))
                    {
                        foreach (BaseBioReactor baseBioReactor in baseBioReactors)
                        {
                            if (baseBioReactor.container.HasRoomFor(parkCreature.pickupable))
                            {
                                creature.ResetBreedTime();
                                parkCreature.ResetBreedTime();
                                GameObject gameObject = CraftData.InstantiateFromPrefab(CraftData.GetTechType(parkCreature.gameObject), false);
                                gameObject.SetActive(false);
                                baseBioReactor.container.AddItem(gameObject.EnsureComponent<Pickupable>());
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(WaterParkCreature))]
        [HarmonyPatch(nameof(WaterParkCreature.Born))]
        internal class WaterParkCreature_Born_Prefix
        {
            [HarmonyPrefix]
            public static bool Prefix(WaterParkCreature __instance, WaterPark waterPark, Vector3 position)
            {
                return waterPark.IsPointInside(position);
            }
        }

    }
}
