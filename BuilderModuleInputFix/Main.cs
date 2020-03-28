using Harmony;
using System;
using System.Reflection;
using UnityEngine;

namespace BuilderModuleInputFix
{
    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch("Update")]
    internal class Builder_Update_Patch
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            if (Player.main.GetVehicle() != null)
            {
                Builder.Initialize();
                Builder.canPlace = false;
                if (Builder.prefab == null)
                {
                    return true;
                }
                if (!Builder.CreateGhost())
                {
                    Builder.inputHandler.canHandleInput = false;
                }
                Builder.canPlace = Builder.UpdateAllowed();
                Transform transform = Builder.ghostModel.transform;
                transform.position = Builder.placePosition + Builder.placeRotation * Builder.ghostModelPosition;
                transform.rotation = Builder.placeRotation * Builder.ghostModelRotation;
                transform.localScale = Builder.ghostModelScale;
                Color value = (!Builder.canPlace) ? Builder.placeColorDeny : Builder.placeColorAllow;
                IBuilderGhostModel[] components = Builder.ghostModel.GetComponents<IBuilderGhostModel>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].UpdateGhostModelColor(Builder.canPlace, ref value);
                }
                Builder.ghostStructureMaterial.SetColor(ShaderPropertyID._Tint, value);

                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch("Construct")]
    internal class Construct_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(Constructable __instance)
        {
            if (Player.main.GetVehicle() != null)
            {
                var thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return false;
                }
                int count = __instance.resourceMap.Count;
                int resourceID = __instance.GetResourceID();
                var backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount += Time.deltaTime / ((float)count * Constructable.GetConstructInterval());
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = __instance.GetResourceID();
                if (resourceID2 != resourceID)
                {
                    TechType destroyTechType = __instance.resourceMap[resourceID2 - 1];
                    if (thisVehicle.GetType().Equals(typeof(Exosuit)))
                    {
                        var storageContainer = ((Exosuit)thisVehicle).storageContainer;

                        if (storageContainer.container.Contains(destroyTechType) && GameModeUtils.RequiresIngredients())
                        {
                            storageContainer.container.DestroyItem(destroyTechType);
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
                                var storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                if (storage != null && storage.Contains(destroyTechType) && GameModeUtils.RequiresIngredients())
                                {
                                    storage.DestroyItem(destroyTechType);
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
                if (__instance.constructedAmount >= 1f)
                {
                    __instance.SetState(true, true);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch("Deconstruct")]
    internal class Deconstruct_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(Constructable __instance)
        {
            if (Player.main.GetVehicle() != null)
            {
                var thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return true;
                }
                int count = __instance.resourceMap.Count;
                int resourceID = __instance.GetResourceID();
                var backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount -= Time.deltaTime / ((float)count * Constructable.GetConstructInterval());
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = __instance.GetResourceID();
                if (resourceID2 != resourceID && GameModeUtils.RequiresIngredients())
                {
                    TechType techType = __instance.resourceMap[resourceID2];
                    GameObject gameObject = CraftData.InstantiateFromPrefab(techType, false);
                    Pickupable component = gameObject.GetComponent<Pickupable>();

                    if (thisVehicle.GetType().Equals(typeof(Exosuit)))
                    {
                        var storageContainer = ((Exosuit)thisVehicle).storageContainer;

                        if (storageContainer.container.HasRoomFor(component) && GameModeUtils.RequiresIngredients())
                        {
                            var name = Language.main.Get(component.GetTechName());
                            ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));

                            uGUI_IconNotifier.main.Play(component.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);

                            component = component.Initialize();

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
                                var storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                if (storage != null && storage.HasRoomFor(component) && GameModeUtils.RequiresIngredients())
                                {
                                    var name = Language.main.Get(component.GetTechName());
                                    ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));

                                    uGUI_IconNotifier.main.Play(component.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);

                                    component = component.Initialize();

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
                if (__instance.constructedAmount <= 0f)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
