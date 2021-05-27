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
    public static class WaterParkUpdatePostfix
    {
        private static readonly Dictionary<WaterPark, List<WaterParkItem>> CachedItems = new();
        private static readonly Dictionary<WaterPark, List<WaterParkItem>> CachedPowerCreatures = new();

        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance)
        {
            if (!Main.Config.EnablePowerGeneration) return;

            var powerCreatures = new List<WaterParkItem>();
            float maxPower = 0;
            if(CachedItems.TryGetValue(__instance, out var items) && items.Count == __instance.items.Count && CachedPowerCreatures.ContainsKey(__instance))
            {
                powerCreatures.AddRange(CachedPowerCreatures[__instance]);

                foreach(var creature in CachedPowerCreatures[__instance])
                {
                    if(!creature.gameObject.TryGetComponent(out LiveMixin liveMixin) || !liveMixin.IsAlive())
                        powerCreatures.Remove(creature);
                }

                maxPower += Main.Config.CreaturePowerGeneration
                    .Select(pair => new
                    {
                        pair, creatures = __instance.items.FindAll(item => item.pickupable.GetTechType() == pair.Key)
                    })
                    .Where(t => t.creatures.Count > 0)
                    .Select(selector: t => 50 * t.pair.Value * t.creatures.Count).Sum();

                CachedPowerCreatures[__instance] = powerCreatures;
            }
            else
            {
                foreach(var pair in Main.Config.CreaturePowerGeneration)
                {
                    var creatures = __instance.items.FindAll(item => item.pickupable.GetTechType() == pair.Key && item.GetComponent<LiveMixin>() != null && item.GetComponent<LiveMixin>().IsAlive());
                    if (creatures.Count <= 0) continue;

                    maxPower += 50 * pair.Value * creatures.Count;
                    powerCreatures.AddRange(creatures);
                }

                CachedItems[__instance] = __instance.items;
                CachedPowerCreatures[__instance] = powerCreatures;
            }

            var rootObject = __instance.itemsRoot.gameObject;

            var powerSource = rootObject.GetComponent<PowerSource>();
            if(powerSource is null)
            {
                powerSource = rootObject.AddComponent<PowerSource>();
                powerSource.maxPower = maxPower;
                powerSource.power = Main.Config.PowerValues.GetOrDefault($"PowerSource:{__instance.GetInstanceID()}", 0f);
            }
            else
            {
                powerSource.maxPower = maxPower;

                if(powerSource.power > powerSource.maxPower)
                    powerSource.power = powerSource.maxPower;

                Main.Config.PowerValues[$"PowerSource:{__instance.GetInstanceID()}"] = powerSource.power;
            }
        }
    }

    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.HasFreeSpace))]
    internal class WaterParkHasFreeSpacePostfix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterPark __instance)
        {
            __instance.wpPieceCapacity = Main.Config.WaterParkSize;
        }
    }
#if BZ
    [HarmonyPatch(typeof(LargeRoomWaterPark), nameof(LargeRoomWaterPark.HasFreeSpace))]
    internal class LargeRoomWaterPark_HasFreeSpace_Postfix
    {
        [HarmonyPrefix]
        public static void Prefix(WaterPark __instance)
        {
            __instance.wpPieceCapacity = Main.Config.LargeWaterParkSize;
        }
    }
#endif

#if SN1
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.TryBreed))]
#elif BZ
    [HarmonyPatch(typeof(WaterPark), nameof(WaterPark.GetBreedingPartner))]
#endif
    internal class WaterParkBreedPostfix
    {
        [HarmonyPostfix]
        public static void Postfix(WaterPark __instance, WaterParkCreature creature)
        {
            if (!Main.Config.AlterraGenOverflow && !Main.Config.BioReactorOverflow && !Main.Config.OceanBreeding) return;

            var items = __instance.items;
            var techType = creature.pickupable.GetTechType();
            if(!items.Contains(creature) || __instance.HasFreeSpace() || BaseBioReactor.GetCharge(techType) <= 0f)
                return;

            var hasBred = false;
            foreach(var waterParkItem in items)
            {
                var parkCreature = waterParkItem as WaterParkCreature;
                var parkCreatureTechType = parkCreature is not null && parkCreature.pickupable != null ? parkCreature.pickupable.GetTechType() : TechType.None;
                if (parkCreature == null || parkCreature == creature || !parkCreature.GetCanBreed() ||
                    parkCreatureTechType != techType || parkCreatureTechType.ToString().Contains("Egg"))
                {
                    continue;
                }

                if(BaseBioReactor.GetCharge(parkCreatureTechType) > -1)
                {
                    if(Main.Config.AlterraGenOverflow && !Main.Config.AlterraGenBlackList.Contains(parkCreatureTechType) && QModServices.Main.ModPresent("FCSEnergySolutions"))
                    {
                        hasBred = AGT.TryBreedIntoAlterraGen(__instance, parkCreatureTechType, parkCreature);
                    }

                    if(Main.Config.BioReactorOverflow && !Main.Config.BioReactorBlackList.Contains(parkCreatureTechType) && !hasBred)
                    {
                        var baseBioReactors =
                            __instance.gameObject.GetComponentInParent<SubRoot>()?.gameObject
                                .GetComponentsInChildren<BaseBioReactor>()
                                ?.Where(baseBioReactor => baseBioReactor.container.HasRoomFor(parkCreature.pickupable))
                                .ToList() ?? new List<BaseBioReactor>();

                        if (baseBioReactors.Count > 0)
                        {
                            hasBred = true;
                            baseBioReactors.Shuffle();
                            var baseBioReactor = baseBioReactors.First();
                            CoroutineHost.StartCoroutine(SpawnCreature(__instance, parkCreatureTechType, baseBioReactor.container));
                        }
                    }

                    if(Main.Config.OceanBreeding && Main.Config.OceanBreedWhiteList.Contains(parkCreatureTechType) && !hasBred && __instance.transform.position.y < 0)
                    {
                        CoroutineHost.StartCoroutine(SpawnCreature(__instance, parkCreatureTechType, null));
                        hasBred = true;
                    }
                }

                if (hasBred)
                {
                    creature.ResetBreedTime();
                    parkCreature.ResetBreedTime();
                }
                break;
            }
        }

        private static IEnumerator SpawnCreature(WaterPark waterPark, TechType parkCreatureTechType, ItemsContainer container)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(parkCreatureTechType, false);
            yield return task;

            var prefab = task.GetResult();
            if (prefab == null) yield break;

            prefab.SetActive(false);
            var gameObject = Object.Instantiate(prefab);

            if(container is not null)
            {
                var pickupable = gameObject.EnsureComponent<Pickupable>();
#if SUBNAUTICA_EXP
                    TaskResult<Pickupable> taskResult = new TaskResult<Pickupable>();
                    yield return pickupable.PickupAsync(taskResult, false);
                    pickupable = taskResult.Get();
#else
                pickupable.Pickup(false);
#endif
                gameObject.SetActive(false);
                container.AddItem(pickupable);
                yield break;
            }

            var spawnPoint = waterPark.transform.position + (Random.insideUnitSphere * 50);
            var @base =
#if SN1
                waterPark.GetComponentInParent<Base>();
#elif BZ
                    waterPark.hostBase;
#endif

            while(Vector3.Distance(@base.GetClosestPoint(spawnPoint), spawnPoint) < 25 || spawnPoint.y >= 0)
            {
                yield return null;
                spawnPoint = @base.GetClosestPoint(spawnPoint) + (Random.insideUnitSphere * 50);
            }

            gameObject.transform.SetPositionAndRotation(spawnPoint, Quaternion.identity);
            gameObject.SetActive(true);
        }
    }
}
