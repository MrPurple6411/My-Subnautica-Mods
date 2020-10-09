using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UWE;
using Random = UnityEngine.Random;

namespace BetterACU.Patches
{
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.Update))]
    public static class WaterPark_Update_Postfix
    {
        public static Dictionary<WaterPark, int> cachedItems = new Dictionary<WaterPark, int>();
        public static Dictionary<WaterPark, int> cachedPowerCreatures = new Dictionary<WaterPark, int>();

        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance)
        {
            int numberOfPowerCreatures = 0;

            if (cachedItems.TryGetValue(__instance, out int count) && count == __instance.items.Count && cachedPowerCreatures.ContainsKey(__instance))
            {
                numberOfPowerCreatures = cachedPowerCreatures[__instance];
            }
            else
            {
                foreach (TechType techType in Main.config.CreaturePowerGeneration.Keys)
                {
                    numberOfPowerCreatures += __instance.items.FindAll((WaterParkItem item) => item.pickupable.GetTechType() == techType && (item.GetComponent<LiveMixin>()?.IsAlive() ?? false)).Count;
                }

                cachedItems[__instance] = __instance.items.Count;
                cachedPowerCreatures[__instance] = numberOfPowerCreatures;
            }

            PowerSource powerSource = __instance.gameObject.GetComponent<PowerSource>();
            if (powerSource == null)
            {
                powerSource = __instance.gameObject.AddComponent<PowerSource>();
                powerSource.maxPower = 500 * numberOfPowerCreatures;
                powerSource.power = Main.config.PowerValues.GetOrDefault($"PowerSource:{__instance.GetInstanceID()}",  0f);
            }
            else
            {
                powerSource.maxPower = 500 * numberOfPowerCreatures;
            }

            if (powerSource.power > powerSource.maxPower)
            {
                powerSource.power = powerSource.maxPower;
            }

            Main.config.PowerValues[$"PowerSource:{__instance.GetInstanceID()}"] = powerSource.power;
        }
    }

    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.HasFreeSpace))]
    internal class WaterPark_HasFreeSpace_Postfix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterPark __instance)
        {
#if BZ
            if(__instance is LargeRoomWaterPark)
            {
                __instance.wpPieceCapacity = Main.config.LargeWaterParkSize;
            }
            else
#endif
                __instance.wpPieceCapacity= Main.config.WaterParkSize;
        }
    }

#if SN1
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.TryBreed))]
    internal class WaterPark_TryBreed_Prefix
    {
        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance, WaterParkCreature creature)
        {
            List<WaterParkItem> items = __instance.items;
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
                        if (baseBioReactor.container.HasRoomFor(parkCreature.pickupable))
                        {
                            creature.ResetBreedTime();
                            parkCreature.ResetBreedTime();
                            hasBred = true;
                            CoroutineHost.StartCoroutine(SpawnCreature(__instance, parkCreature, baseBioReactor.container));
                            break;
                        }
                    }

                    if (!hasBred && Main.config.OverFlowIntoOcean)
                    {
                        CoroutineHost.StartCoroutine(SpawnCreature(__instance, parkCreature, null));
                        break;
                    }

                    creature.ResetBreedTime();
                    parkCreature.ResetBreedTime();
                    break;
                }
            }
        }

        private static IEnumerator SpawnCreature(WaterPark waterPark, WaterParkCreature parkCreature, ItemsContainer container)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(parkCreature.pickupable.GetTechType(), false);
            yield return task;

            GameObject prefab = task.GetResult();
            prefab.SetActive(false);
            GameObject gameObject = GameObject.Instantiate(prefab);
            
            if(container is null)
            {
                gameObject.transform.position = waterPark.gameObject.GetComponentInParent<SubRoot>().transform.up + new Vector3(Random.Range(-100, 100), Random.Range(2, 30), Random.Range(-100, 100));
                gameObject.SetActive(true);
            }
            else
            {
                Pickupable pickupable = gameObject.EnsureComponent<Pickupable>();
#if SUBNAUTICA_EXP
                TaskResult<Pickupable> taskResult = new TaskResult<Pickupable>();
                yield return pickupable.PickupAsync(taskResult, false);

                container.AddItem(taskResult.Get());
#else
                pickupable.Pickup(false);

                container.AddItem(pickupable);
#endif
            }

            yield break;
        }
    }
#elif BZ
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.GetBreedingPartner))]
    internal class WaterPark_GetBreedingPartner_Postfix
    {
        private static int count = 0;

        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance, WaterParkCreature creature)
        {
            List<WaterParkItem> items = __instance.items;
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
                    WaterParkCreatureData data = parkCreature.data;
                    foreach (BaseBioReactor baseBioReactor in baseBioReactors)
                    {
                        if (baseBioReactor.container.HasRoomFor(parkCreature.pickupable))
                        {
                            creature.ResetBreedTime();
                            parkCreature.ResetBreedTime();
                            GameObject gameObject = CraftData.InstantiateFromPrefab(CraftData.GetTechType(data.eggOrChildPrefab), false);
                            gameObject.SetActive(false);
                            baseBioReactor.container.AddItem(gameObject.EnsureComponent<Pickupable>());
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
