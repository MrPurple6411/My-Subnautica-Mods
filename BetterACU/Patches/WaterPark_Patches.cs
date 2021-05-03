namespace BetterACU.Patches
{
    using HarmonyLib;
    using QModManager.API;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UWE;

    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.Update))]
    public static class WaterPark_Update_Postfix
    {
        public static Dictionary<WaterPark, List<WaterParkItem>> cachedItems = new Dictionary<WaterPark, List<WaterParkItem>>();
        public static Dictionary<WaterPark, List<WaterParkItem>> cachedPowerCreatures = new Dictionary<WaterPark, List<WaterParkItem>>();
        public static float timeSinceLastPositionCheck = 0;

        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance)
        {
            List<WaterParkItem> powerCreatures = new List<WaterParkItem>();
            float maxPower = 0;
            if(cachedItems.TryGetValue(__instance, out List<WaterParkItem> items) && items.Count == __instance.items.Count && cachedPowerCreatures.ContainsKey(__instance))
            {
                powerCreatures.AddRange(cachedPowerCreatures[__instance]);

                foreach(WaterParkItem creature in cachedPowerCreatures[__instance])
                {
                    if(!creature.gameObject.TryGetComponent(out LiveMixin liveMixin) || !liveMixin.IsAlive())
                        powerCreatures.Remove(creature);
                }

                foreach(KeyValuePair<TechType, float> pair in Main.Config.CreaturePowerGeneration)
                {
                    List<WaterParkItem> creatures = __instance.items.FindAll((WaterParkItem item) => item.pickupable.GetTechType() == pair.Key) ?? new List<WaterParkItem>();
                    if(creatures.Count > 0)
                        maxPower += 500 * pair.Value * creatures.Count;
                }

                cachedPowerCreatures[__instance] = powerCreatures;
            }
            else
            {
                foreach(KeyValuePair<TechType, float> pair in Main.Config.CreaturePowerGeneration)
                {
                    List<WaterParkItem> creatures = __instance.items.FindAll((WaterParkItem item) => item.pickupable.GetTechType() == pair.Key && (item.GetComponent<LiveMixin>()?.IsAlive() ?? false)) ?? new List<WaterParkItem>();
                    if(creatures.Count > 0)
                    {
                        maxPower += 500 * pair.Value * creatures.Count;
                        powerCreatures.AddRange(creatures);
                    }
                }

                cachedItems[__instance] = __instance.items;
                cachedPowerCreatures[__instance] = powerCreatures;
            }

            PowerSource powerSource = __instance?.itemsRoot?.gameObject?.GetComponent<PowerSource>();
            if(powerSource is null)
            {
                powerSource = __instance?.itemsRoot?.gameObject?.AddComponent<PowerSource>();
                powerSource.maxPower = maxPower;
                powerSource.power = Main.Config.PowerValues.GetOrDefault($"PowerSource:{__instance.GetInstanceID()}", 0f);
            }
            else
            {
                if(powerSource.maxPower != maxPower)
                    powerSource.maxPower = maxPower;

                if(powerSource.power > powerSource.maxPower)
                    powerSource.power = powerSource.maxPower;

                Main.Config.PowerValues[$"PowerSource:{__instance.GetInstanceID()}"] = powerSource.power;
            }

            timeSinceLastPositionCheck += Time.deltaTime;
            if(timeSinceLastPositionCheck > 0.5f)
            {
                foreach(WaterParkItem waterParkItem in __instance.items)
                {
                    if(!__instance.IsPointInside(waterParkItem.transform.position))
                    {
                        Vector3 position = waterParkItem.transform.position;
                        __instance.EnsurePointIsInside(ref position);
                        waterParkItem.transform.position = position;
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
                __instance.wpPieceCapacity = Main.Config.LargeWaterParkSize;
            }
            else
#endif
            __instance.wpPieceCapacity = Main.Config.WaterParkSize;
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
            if(!items.Contains(creature) || __instance.HasFreeSpace() || BaseBioReactor.GetCharge(creature.pickupable.GetTechType()) == -1)
            {
                return;
            }

            List<BaseBioReactor> baseBioReactors = __instance.gameObject.GetComponentInParent<SubRoot>().gameObject.GetComponentsInChildren<BaseBioReactor>().ToList();
            bool hasBred = false;
            foreach(WaterParkItem waterParkItem in items)
            {
                WaterParkCreature parkCreature = waterParkItem as WaterParkCreature;
                TechType parkCreatureTechType = parkCreature?.pickupable?.GetTechType() ?? TechType.None;
                if(parkCreature != null && parkCreature != creature && parkCreature.GetCanBreed() && parkCreatureTechType == creature.pickupable.GetTechType() && !parkCreatureTechType.ToString().Contains("Egg"))
                {
                    if(BaseBioReactor.GetCharge(parkCreatureTechType) > -1)
                    {
                        if(QModServices.Main.ModPresent("FCSEnergySolutions"))
                        {
                            hasBred = AGT.TryBreedIntoAlterraGen(__instance, parkCreatureTechType, parkCreature);
                        }

                        if(!hasBred)
                        {
                            foreach(BaseBioReactor baseBioReactor in baseBioReactors)
                            {
                                if(baseBioReactor.container.HasRoomFor(parkCreature.pickupable))
                                {
                                    creature.ResetBreedTime();
                                    parkCreature.ResetBreedTime();
                                    hasBred = true;
                                    CoroutineHost.StartCoroutine(SpawnCreature(__instance, parkCreature, baseBioReactor.container));
                                    break;
                                }
                            }

                        }
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

            Pickupable pickupable = gameObject.EnsureComponent<Pickupable>();
#if SUBNAUTICA_EXP
                TaskResult<Pickupable> taskResult = new TaskResult<Pickupable>();
                yield return pickupable.PickupAsync(taskResult, false);
                pickupable = taskResult.Get();
#else
            pickupable.Pickup(false);
#endif
            container.AddItem(pickupable);

            yield break;
        }
    }
}
