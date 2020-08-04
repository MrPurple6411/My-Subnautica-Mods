using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BetterACU
{
    [HarmonyPatch(typeof(WaterPark), "Update")]
    internal class WaterPark_Update_Postfix
    {
        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance)
        {
            List<WaterParkItem> items = AccessTools.Field(typeof(WaterPark), "items").GetValue(__instance) as List<WaterParkItem>;
            PowerSource powerSource = __instance.gameObject.GetComponent<PowerSource>();
            int numberOfShockers = items.FindAll((WaterParkItem item) => item.pickupable.GetTechType() == TechType.Shocker).Count;

            if (powerSource == null)
            {
                powerSource = __instance.gameObject.AddComponent<PowerSource>();
                powerSource.maxPower = 100 * numberOfShockers;
                powerSource.power = Main.config.PowerValues.GetOrDefault($"PowerSource:{__instance.GetInstanceID()}",  0f);
            }
            else
            {
                powerSource.maxPower = 100 * numberOfShockers;
            }
            if (powerSource.GetPower() > powerSource.GetMaxPower())
            {
                powerSource.power = powerSource.maxPower;
            }

            Main.config.PowerValues[$"PowerSource:{__instance.GetInstanceID()}"] = powerSource.GetPower();
        }
    }

    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.HasFreeSpace))]
    internal class WaterPark_HasFreeSpace_Postfix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterPark __instance)
        {
#if BELOWZERO
            if(__instance is LargeRoomWaterPark)
            {
                AccessTools.Field(typeof(WaterPark), "wpPieceCapacity").SetValue(__instance, Main.config.LargeWaterParkSize);
            }
            else
#endif
            AccessTools.Field(typeof(WaterPark), "wpPieceCapacity").SetValue(__instance, Main.config.WaterParkSize);
        }
    }

#if SUBNAUTICA
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.TryBreed))]
    internal class WaterPark_TryBreed_Prefix
    {
        private static int count = 0;

        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance, WaterParkCreature creature)
        {
            List<WaterParkItem> items = AccessTools.Field(typeof(WaterPark), "items").GetValue(__instance) as List<WaterParkItem>;
            if (!items.Contains(creature) || __instance.HasFreeSpace())
            {
                return;
            }

            var baseBioReactors = __instance.gameObject.GetComponentInParent<SubRoot>().gameObject.GetComponentsInChildren<BaseBioReactor>().ToList();
            bool hasBred = false;
            foreach (WaterParkItem waterParkItem in items)
            {
                var parkCreature = waterParkItem as WaterParkCreature;
                if (parkCreature != null && parkCreature != creature && parkCreature.GetCanBreed() && parkCreature.pickupable.GetTechType() == creature.pickupable.GetTechType() && !parkCreature.pickupable.GetTechType().ToString().Contains("Egg"))
                {
                    foreach (BaseBioReactor baseBioReactor in baseBioReactors)
                    {
                        ItemsContainer container = AccessTools.Property(typeof(BaseBioReactor), "container").GetValue(baseBioReactor) as ItemsContainer;
                        if (container.HasRoomFor(parkCreature.pickupable))
                        {
                            creature.ResetBreedTime();
                            parkCreature.ResetBreedTime();
                            GameObject gameObject = CraftData.InstantiateFromPrefab(WaterParkCreature.creatureEggs.GetOrDefault(parkCreature.pickupable.GetTechType(), parkCreature.pickupable.GetTechType()), false);
                            gameObject.SetActive(false);
                            container.AddItem(gameObject.EnsureComponent<Pickupable>());
                            hasBred = true;
                            break;
                        }
                    }
                    if (!hasBred && Main.config.OverFlowIntoOcean && (!WaterParkCreature.waterParkCreatureParameters.ContainsKey(parkCreature.pickupable.GetTechType()) || parkCreature.pickupable.GetTechType() == TechType.Spadefish))
                    {
                        creature.ResetBreedTime();
                        parkCreature.ResetBreedTime();
                        if (count > Main.config.WaterParkSize)
                        {
                            GameObject gameObject = CraftData.InstantiateFromPrefab(CraftData.GetTechType(parkCreature.gameObject), false);
                            gameObject.transform.position = __instance.gameObject.GetComponentInParent<SubRoot>().transform.position + new Vector3(Random.Range(-30, 30), Random.Range(-2, 30), Random.Range(-30, 30));
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
#elif BELOWZERO
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.GetBreedingPartner))]
    internal class WaterPark_GetBreedingPartner_Postfix
    {
        private static int count = 0;

        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance, WaterParkCreature creature)
        {
            List<WaterParkItem> items = AccessTools.Field(typeof(WaterPark), "items").GetValue(__instance) as List<WaterParkItem>;
            if (!items.Contains(creature) || __instance.HasFreeSpace())
            {
                return;
            }

            List<BaseBioReactor> baseBioReactors = __instance.gameObject.GetComponentInParent<SubRoot>().gameObject.GetComponentsInChildren<BaseBioReactor>().ToList();
            bool hasBred = false;
            foreach(WaterParkItem waterParkItem in items)
            {
                WaterParkCreature parkCreature = waterParkItem as WaterParkCreature;
                if(parkCreature != null && parkCreature != creature && parkCreature.GetCanBreed() && parkCreature.pickupable.GetTechType() == creature.pickupable.GetTechType() && !parkCreature.pickupable.GetTechType().ToString().Contains("Egg"))
                {
                    WaterParkCreatureData data = AccessTools.Field(typeof(WaterParkCreature), "data").GetValue(parkCreature) as WaterParkCreatureData;
                    foreach (BaseBioReactor baseBioReactor in baseBioReactors)
                    {
                        ItemsContainer container = AccessTools.Property(typeof(BaseBioReactor), "container").GetValue(baseBioReactor) as ItemsContainer;
                        if (container.HasRoomFor(parkCreature.pickupable))
                        {
                            creature.ResetBreedTime();
                            parkCreature.ResetBreedTime();
                            GameObject gameObject = CraftData.InstantiateFromPrefab(CraftData.GetTechType(data.eggOrChildPrefab), false);
                            gameObject.SetActive(false);
                            container.AddItem(gameObject.EnsureComponent<Pickupable>());
                            hasBred = true;
                            break;
                        }
                    }
                    if(!hasBred && Main.config.OverFlowIntoOcean && data.isPickupableOutside)
                    {
                        creature.ResetBreedTime();
                        parkCreature.ResetBreedTime();
                        if(count > Main.config.WaterParkSize)
                        {
                            GameObject gameObject = CraftData.InstantiateFromPrefab(CraftData.GetTechType(parkCreature.gameObject), false);
                            gameObject.transform.position = __instance.gameObject.GetComponentInParent<SubRoot>().transform.position + new Vector3(Random.Range(-30, 30), Random.Range(-2, 30), Random.Range(-30, 30));
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
#endif
}
