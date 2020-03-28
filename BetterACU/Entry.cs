using Harmony;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BetterACU
{
    [QModCore]
    public class Entry
    {
        [QModPatch]
        public static void Patch()
        {
            Config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());
            HarmonyInstance.Create("MrPurple6411_BetterACU").PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public static class Config
    {
        public static int WaterParkSize;
        public static bool OverFlowIntoOcean;
        public static Dictionary<string, float> PowerValues = new Dictionary<string, float>();

        public static void Load()
        {
            WaterParkSize = PlayerPrefs.GetInt("WaterParkSize", 10);
            OverFlowIntoOcean = PlayerPrefs.GetInt("OverFlowIntoOcean", 0) < 1;

            IngameMenuHandler.RegisterOnSaveEvent(()=> PowerValues.ForEach((KeyValuePair<string, float> keyValuePair) => PlayerPrefs.SetFloat(keyValuePair.Key, keyValuePair.Value)));
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Better ACU")
        {
            SliderChanged += BetterACU_SliderChanged;
            ToggleChanged += BetterACU_ToggleChanged;
        }

        private void BetterACU_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            List<string> ids = new List<string>() { "OverFlowIntoOcean" };

            if (!ids.Contains(e.Id))
            {
                return;
            }

            switch (e.Id)
            {
                case "OverFlowIntoOcean":
                    Config.OverFlowIntoOcean = e.Value;
                    PlayerPrefs.SetInt("OverFlowIntoOcean", e.Value? 0 : 1);
                    break;

                default:
                    break;
            }
        }

        public void BetterACU_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            List<string> ids = new List<string>() { "WaterParkSize" };

            if (!ids.Contains(e.Id))
            {
                return;
            }

            switch (e.Id)
            {
                case "WaterParkSize":
                    Config.WaterParkSize = (int)e.Value;
                    PlayerPrefs.SetInt("WaterParkSize", (int)e.Value);
                    break;

                default:
                    break;
            }
        }

        public override void BuildModOptions()
        {
            AddSliderOption("WaterParkSize", "Alien Containment Limit", 10, 100, Config.WaterParkSize);
            AddToggleOption("OverFlowIntoOcean", "Allow Breed Into Ocean", Config.OverFlowIntoOcean);
        }
    }


    [HarmonyPatch(typeof(WaterPark))]
    [HarmonyPatch(nameof(WaterPark.Update))]
    internal class WaterPark_Update_Postfix
    {
        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance)
        {
            PowerSource powerSource = __instance.gameObject.GetComponent<PowerSource>();
            int numberOfShockers = __instance.items.FindAll((WaterParkItem item) => item.pickupable.GetTechType() == TechType.Shocker).Count;

            if (powerSource == null)
            {
                powerSource = __instance.gameObject.AddComponent<PowerSource>();
                powerSource.maxPower = (float)(100 * numberOfShockers);
                powerSource.power = PlayerPrefs.GetFloat($"PowerSource:{__instance.GetInstanceID()}", 0f);
            }
            else
            {
                powerSource.maxPower = (float)(100 * numberOfShockers);
            }
            if (powerSource.GetPower() > powerSource.GetMaxPower())
                powerSource.power = powerSource.maxPower;

            Config.PowerValues[$"PowerSource:{__instance.GetInstanceID()}"] = powerSource.GetPower();
        }
    }

    [HarmonyPatch(typeof(WaterPark))]
    [HarmonyPatch(nameof(WaterPark.HasFreeSpace))]
    internal class WaterPark_HasFreeSpace_Postfix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterPark __instance)
        {
            __instance.wpPieceCapacity = Config.WaterParkSize;
        }
    }

    [HarmonyPatch(typeof(WaterPark))]
    [HarmonyPatch(nameof(WaterPark.TryBreed))]
    internal class WaterPark_TryBreed_Prefix
    {
        private static int count = 0;

        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance, WaterParkCreature creature)
        {
            if (!__instance.items.Contains(creature) || __instance.HasFreeSpace()) return;

            List<BaseBioReactor> baseBioReactors = __instance.gameObject.GetComponentInParent<SubRoot>().gameObject.GetComponentsInChildren<BaseBioReactor>().ToList();
            bool hasBred = false;
            foreach (WaterParkItem waterParkItem in __instance.items)
            {
                WaterParkCreature parkCreature = waterParkItem as WaterParkCreature;
                if (parkCreature != null && parkCreature != creature && parkCreature.GetCanBreed() && parkCreature.pickupable.GetTechType() == creature.pickupable.GetTechType() && !parkCreature.pickupable.GetTechType().ToString().Contains("Egg"))
                {
                    foreach (BaseBioReactor baseBioReactor in baseBioReactors)
                    {
                        if (baseBioReactor.container.HasRoomFor(parkCreature.pickupable))
                        {
                            creature.ResetBreedTime();
                            parkCreature.ResetBreedTime();
                            GameObject gameObject = CraftData.InstantiateFromPrefab(WaterParkCreature.creatureEggs.GetOrDefault(parkCreature.pickupable.GetTechType(), parkCreature.pickupable.GetTechType()), false);
                            gameObject.SetActive(false);
                            baseBioReactor.container.AddItem(gameObject.EnsureComponent<Pickupable>());
                            hasBred = true;
                            break;
                        }
                    }
                    if (!hasBred && Config.OverFlowIntoOcean && (!WaterParkCreature.waterParkCreatureParameters.ContainsKey(parkCreature.pickupable.GetTechType())|| parkCreature.pickupable.GetTechType() == TechType.Spadefish))
                    {
                        creature.ResetBreedTime();
                        parkCreature.ResetBreedTime();
                        if (count > Config.WaterParkSize)
                        {
                            GameObject gameObject = CraftData.InstantiateFromPrefab(CraftData.GetTechType(parkCreature.gameObject), false);
                            gameObject.transform.position = (__instance.gameObject.GetComponentInParent<SubRoot>().transform.position + new Vector3(Random.Range(-30, 30), Random.Range(-2,30), Random.Range(-30, 30)));
                            gameObject.SetActive(true);
                            count = 0;
                        }
                        else
                        {
                            count++;
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
        public static bool Prefix(WaterPark waterPark, Vector3 position)
        {
            return waterPark.IsPointInside(position);
        }
    }

    [HarmonyPatch(typeof(WaterParkCreature))]
    [HarmonyPatch(nameof(WaterParkCreature.Update))]
    internal class WaterParkCreature_Update_Prefix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterParkCreature __instance)
        {
            if (__instance.pickupable.GetTechType() == TechType.Shocker && __instance.GetCanBreed() && DayNightCycle.main.timePassed > (double)__instance.timeNextBreed)
            {
                __instance.GetWaterPark()?.gameObject.GetComponent<PowerSource>()?.AddEnergy(100f, out _);
            }
        }
    }

    [HarmonyPatch(typeof(CreatureEgg))]
    [HarmonyPatch(nameof(CreatureEgg.Hatch))]
    internal class CreatureEgg_Hatch_Prefix
    {
        [HarmonyPostfix]
        public static void Postfix(CreatureEgg __instance)
        {
            UnityEngine.Object.Destroy(__instance.gameObject);
        }
    }
}
