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
        public static float timeSinceLastPositionCheck = 0;

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

            PowerSource powerSource = __instance?.itemsRoot?.gameObject?.GetComponent<PowerSource>();
            if (powerSource is null)
            {
                powerSource = __instance?.itemsRoot?.gameObject?.AddComponent<PowerSource>();
                powerSource.maxPower = 500 * numberOfPowerCreatures;
                powerSource.power = Main.config.PowerValues.GetOrDefault($"PowerSource:{__instance.GetInstanceID()}",  0f);
            }
            else
            {

                if (powerSource.maxPower != 500 * numberOfPowerCreatures)
                    powerSource.maxPower = 500 * numberOfPowerCreatures;

                if (powerSource.power > powerSource.maxPower)
                    powerSource.power = powerSource.maxPower;

                Main.config.PowerValues[$"PowerSource:{__instance.GetInstanceID()}"] = powerSource.power;
            }

            timeSinceLastPositionCheck += Time.deltaTime;
            if(timeSinceLastPositionCheck > 0.5f)
            {
                foreach (WaterParkItem waterParkItem in __instance.items)
                {
                    if (!__instance.IsPointInside(waterParkItem.transform.position))
                    {
                        waterParkItem.transform.position = __instance.GetRandomPointInside();
                    }
                }
                timeSinceLastPositionCheck = 0;
            }
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
                __instance.wpPieceCapacity = Main.config.WaterParkSize;
        }
    }

#if SN1
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.TryBreed))]
#elif BZ
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.GetBreedingPartner))]
#endif
    internal class WaterPark_Breed_Postfix
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
                TechType parkCreatureTechType = parkCreature?.pickupable?.GetTechType() ?? TechType.None;
                if (parkCreature != null && parkCreature != creature && parkCreature.GetCanBreed() && parkCreatureTechType == creature.pickupable.GetTechType() && !parkCreatureTechType.ToString().Contains("Egg"))
                {
                    if(BaseBioReactor.GetCharge(parkCreatureTechType) > -1)
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
                pickupable = taskResult.Get();
#else
                pickupable.Pickup(false);
#endif
                container.AddItem(pickupable);
            }

            yield break;
        }
    }
}
