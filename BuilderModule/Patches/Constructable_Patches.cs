using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

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
                Type ConstructableType = typeof(Constructable);
                List<TechType> resourceMapField = (List<TechType>)AccessTools.Field(ConstructableType, "resourceMap").GetValue(__instance);
                MethodInfo getResourceIDMethod = AccessTools.Method(ConstructableType, "GetResourceID");
                MethodInfo GetConstructIntervalMethod = AccessTools.Method(ConstructableType, "GetConstructInterval");
                MethodInfo updateMaterialMethod = AccessTools.Method(ConstructableType, "UpdateMaterial");


                Vehicle thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return false;
                }
                int count = resourceMapField.Count;
                int resourceID = (int)getResourceIDMethod.Invoke(__instance, null);
                float backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount += Time.deltaTime / (count * (float)GetConstructIntervalMethod.Invoke(__instance, null));
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = (int)getResourceIDMethod.Invoke(__instance, null);
                if (resourceID2 != resourceID)
                {
                    TechType destroyTechType = resourceMapField[resourceID2 - 1];
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
                _ = updateMaterialMethod.Invoke(__instance, null);
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

    [HarmonyPatch(typeof(Constructable),nameof(Constructable.Deconstruct))]
    internal class Deconstruct_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Constructable __instance)
        {
            if (Player.main.GetVehicle() != null && GameModeUtils.RequiresIngredients())
            {
                Type ConstructableType = typeof(Constructable);
                List<TechType> resourceMapField = (List<TechType>)AccessTools.Field(ConstructableType, "resourceMap").GetValue(__instance);
                MethodInfo getResourceIDMethod = AccessTools.Method(ConstructableType, "GetResourceID");
                MethodInfo GetConstructIntervalMethod = AccessTools.Method(ConstructableType, "GetConstructInterval");
                MethodInfo updateMaterialMethod = AccessTools.Method(ConstructableType, "UpdateMaterial");

                Vehicle thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return true;
                }
                int count = resourceMapField.Count;

                int resourceID = (int)getResourceIDMethod.Invoke(__instance, null);
                float backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount -= Time.deltaTime / (count * (float)GetConstructIntervalMethod.Invoke(__instance, null));
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = (int)getResourceIDMethod.Invoke(__instance, null);
                if (resourceID2 != resourceID)
                {
                    TechType techType = resourceMapField[resourceID2];
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
#if SUBNAUTICA
                            component = component.Initialize();
#elif BELOWZERO
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

#if SUBNAUTICA
                                    component = component.Initialize();
#elif BELOWZERO
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
                updateMaterialMethod.Invoke(__instance, null);
                return __instance.constructedAmount <= 0f;
            }
            else
            {
                return true;
            }
        }
    }
}
