using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UWE;

namespace BuilderModule.Patches
{

    [HarmonyPatch(typeof(Constructable), nameof(Constructable.Construct))]
    internal class Construct_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Constructable __instance)
        {
            if (Player.main.GetVehicle() != null && GameModeUtils.RequiresIngredients())
            {
                Vehicle thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return false;
                }
                int count = __instance.resourceMap.Count;
                int resourceID = __instance.GetResourceID();
                float backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount += Time.deltaTime / (count * Constructable.GetConstructInterval());
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = __instance.GetResourceID();
                if (resourceID2 != resourceID)
                {
                    TechType destroyTechType = __instance.resourceMap[resourceID2 - 1];
                    if (thisVehicle.GetType().Equals(typeof(Exosuit)))
                    {
                        StorageContainer storageContainer = ((Exosuit)thisVehicle).storageContainer;

                        if (storageContainer.container.Contains(destroyTechType))
                        {
                            _ = storageContainer.container.DestroyItem(destroyTechType);
                        }
                        else
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                    else
                    {
                        var seamoth = (SeaMoth)thisVehicle;
                        bool storageCheck = false;
                        for (int i = 0; i < 12; i++)
                        {
                            try
                            {
                                ItemsContainer storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                if (storage != null && storage.Contains(destroyTechType))
                                {
                                    if (storage.DestroyItem(destroyTechType))
                                    {
                                        storageCheck = true;
                                        break;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        if (!storageCheck)
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                }
                __instance.UpdateMaterial();
                if (__instance.constructedAmount >= 1f)
                {
                    _ = __instance.SetState(true, true);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
#if SUBNAUTICA_EXP
    [HarmonyPatch(typeof(Constructable), nameof(Constructable.DeconstructAsync))]
    internal class Deconstruct_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Constructable __instance, IOut<bool> result)
        {
#elif SUBNAUTICA_STABLE
    [HarmonyPatch(typeof(Constructable), nameof(Constructable.Deconstruct))]
    internal class Deconstruct_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Constructable __instance)
        {
#endif
#if SN1
            if (Player.main.GetVehicle() != null && GameModeUtils.RequiresIngredients())
            {

                Vehicle thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return true;
                }
                int count = __instance.resourceMap.Count;

                int resourceID = __instance.GetResourceID();
                float backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount -= Time.deltaTime / (count * Constructable.GetConstructInterval());
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = __instance.GetResourceID();
                if (resourceID2 != resourceID)
                {
                    TechType techType = __instance.resourceMap[resourceID2];

                    Vector2int size = CraftData.GetItemSize(techType);

                    if (thisVehicle.GetType().Equals(typeof(Exosuit)))
                    {
                        StorageContainer storageContainer = ((Exosuit)thisVehicle).storageContainer;

                        if (storageContainer.container.HasRoomFor(size.x, size.y))
                        {
                            CoroutineHost.StartCoroutine(AddToVehicle(techType, storageContainer.container));
                        }
                        else
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                    else
                    {
                        SeaMoth seamoth = (SeaMoth)thisVehicle;
                        bool storageCheck = false;
                        for (int i = 0; i < 12; i++)
                        {
                            try
                            {
                                ItemsContainer storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                if (storage != null && storage.HasRoomFor(size.x, size.y))
                                {
                                    CoroutineHost.StartCoroutine(AddToVehicle(techType, storage));
                                    storageCheck = true;
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        if (!storageCheck)
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                }
                __instance.UpdateMaterial();
#if SUBNAUTICA_EXP
                result.Set(__instance.constructedAmount <= 0f);
                return false;
#elif SUBNAUTICA_STABLE
                return __instance.constructedAmount <= 0f;
#endif
            }
            return true;
        }

        private static IEnumerator AddToVehicle(TechType techType, ItemsContainer itemsContainer)
        {
            CoroutineTask<GameObject> coroutineTask = CraftData.GetPrefabForTechTypeAsync(techType, false);
            yield return coroutineTask;

            GameObject gameObject = GameObject.Instantiate(coroutineTask.GetResult(), null);
            Pickupable pickupable = gameObject.EnsureComponent<Pickupable>();

#if SUBNAUTICA_EXP
            TaskResult<Pickupable> result1 = new TaskResult<Pickupable>();
            yield return pickupable.InitializeAsync(result1);
            pickupable = result1.Get();
#elif SUBNAUTICA_STABLE
            pickupable.Initialize();
#endif
            var item = new InventoryItem(pickupable);
            itemsContainer.UnsafeAdd(item);
            string name = Language.main.GetOrFallback(techType.AsString(), techType.AsString());
            ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));
            uGUI_IconNotifier.main.Play(techType, uGUI_IconNotifier.AnimationType.From, null);
            pickupable.PlayPickupSound();

            yield break;
        }
    }
#else
        [HarmonyPatch(typeof(Constructable),nameof(Constructable.Deconstruct))]
    internal class Deconstruct_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Constructable __instance)
        {
            if (Player.main.GetVehicle() != null && GameModeUtils.RequiresIngredients())
            {
                Vehicle thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return true;
                }
                int count = __instance.resourceMap.Count;

                int resourceID = __instance.GetResourceID();
                float backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount -= Time.deltaTime / (count * Constructable.GetConstructInterval());
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = __instance.GetResourceID();
                if (resourceID2 != resourceID)
                {
                    TechType techType = __instance.resourceMap[resourceID2];
                    GameObject gameObject = CraftData.InstantiateFromPrefab(techType, false);
                    Pickupable component = gameObject.GetComponent<Pickupable>();

                    if (thisVehicle.GetType().Equals(typeof(Exosuit)))
                    {
                        StorageContainer storageContainer = ((Exosuit)thisVehicle).storageContainer;

                        if (storageContainer.container.HasRoomFor(component))
                        {
                            string name = Language.main.Get(component.GetTechName());
                            ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));

                            uGUI_IconNotifier.main.Play(component.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);
#if SN1
                            component = component.Initialize();
#elif BZ
                            component.Initialize();
#endif
                            var item = new InventoryItem(component);
                            storageContainer.container.UnsafeAdd(item);
                            component.PlayPickupSound();
                        }
                        else
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                    else
                    {
                        var seamoth = (SeaMoth)thisVehicle;
                        bool storageCheck = false;
                        for (int i = 0; i < 12; i++)
                        {
                            try
                            {
                                ItemsContainer storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                if (storage != null && storage.HasRoomFor(component))
                                {
                                    string name = Language.main.Get(component.GetTechName());
                                    ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));

                                    uGUI_IconNotifier.main.Play(component.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);

#if SN1
                                    component = component.Initialize();
#elif BZ
                                    component.Initialize();
#endif
                                    var item = new InventoryItem(component);
                                    storage.UnsafeAdd(item);
                                    component.PlayPickupSound();
                                    storageCheck = true;
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        if (!storageCheck)
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                }
                __instance.UpdateMaterial();
                return __instance.constructedAmount <= 0f;
            }
            else
            {
                return true;
            }
        }
    }
#endif
}
