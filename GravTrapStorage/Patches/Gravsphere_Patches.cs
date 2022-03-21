using System;
using System.Linq;
using System.Reflection;
using QModManager.API;
using Story;

namespace GravTrapStorage.Patches;

using System.Collections.Generic;
using SMLHelper.V2.Utility;
using UnityEngine;
using HarmonyLib;

[HarmonyPatch(typeof(Gravsphere))]
internal class GravspherePatches
{
    public static readonly Dictionary<Gravsphere, StorageContainer> StorageContainers = new();
    public static Dictionary<Gravsphere, bool> ResetTriggers = new Dictionary<Gravsphere, bool>();

    [HarmonyPatch(typeof(Gravsphere), nameof(Gravsphere.Start))]
    public static void Prefix(Gravsphere __instance)
    {
        StorageContainer storageContainer = __instance.transform.GetChild(0)?.GetComponent<StorageContainer>();
        if (storageContainer != null)
        {
            storageContainer.container.Resize(Main.ConfigFile.Width, Main.ConfigFile.Height);
            StorageContainers[__instance] = storageContainer;
            ResetTriggers[__instance] = false;
        }
    }

    public static MethodInfo CanBeStored;
    
    [HarmonyPatch(typeof(Gravsphere), nameof(Gravsphere.Update))]
    public static void Postfix(Gravsphere __instance)
    {
        if (Inventory.main.GetHeldTool() != __instance ||
            !StorageContainers.TryGetValue(__instance, out StorageContainer storageContainer) ||
            storageContainer == null) return;

        storageContainer.container.SetAllowedTechTypes(new[] {TechType.None});


        bool targetingStorage = false;
        string targetName = "";
        bool targetStorageHasSpace = false;
        if (!storageContainer.GetOpen() && !IngameMenu.main.selected)
        {
            if (Targeting.GetTarget(Player.main.gameObject, Main.ConfigFile.TransferDistance, out var activeTarget,
                    out _))
            {
                GameObject rootTarget = UWE.Utils.GetEntityRoot(activeTarget) ?? activeTarget;

                var targetTechType = CraftData.GetTechType(rootTarget);
                targetName = targetTechType == TechType.None
                    ? rootTarget.name.Replace("(Clone)", "").Replace("-MainPrefab", "")
                    : Language.main.Get(targetTechType);

                var storageContainers = rootTarget.GetComponentsInChildren<StorageContainer>(true).Where((container) =>
                    {
                        if (QModServices.Main.ModPresent("FCSAlterraHub"))
                        {
                            Type containerType = container.GetType();
                            if (containerType.ToString().StartsWith("FCS"))
                            {
                                if (CanBeStored is null)
                                {
                                    CanBeStored = containerType.GetMethod("CanBeStored",
                                        BindingFlags.Public | BindingFlags.Instance);
                                }
                                else
                                {
                                    foreach (ItemsContainer.ItemGroup itemGroup in storageContainer.container._items.Values)
                                    {
                                        InventoryItem item = itemGroup.items.Count > 0 ? itemGroup.items[0] : null;
                                        if(item == null) continue;
                                        TechType techType = item.item.overrideTechUsed
                                            ? item.item.overrideTechType
                                            : item.item.GetTechType();
                                        if ((bool)CanBeStored.Invoke(container, new object[]{ 1, techType }))
                                            return true;
                                    }
                                }
                                return false;
                            }
                        }
                        foreach (ItemsContainer.ItemGroup itemGroup in storageContainer.container._items.Values)
                        {
                            InventoryItem item = itemGroup.items.Count > 0 ? itemGroup.items[0] : null;
                            if (item != null && container.container.IsTechTypeAllowed(item.item.overrideTechUsed
                                    ? item.item.overrideTechType
                                    : item.item.GetTechType()))
                                return true;
                        }

                        return false;
                    }
                ).ToList();
                ItemsContainer targetContainer = (from container in storageContainers
                    where !container.container.IsFull()
                    select container.container).FirstOrDefault();

                if (storageContainers.Any())
                {
                    targetingStorage = true;

                    if (targetContainer != null && !targetContainer.IsFull())
                    {
                        targetStorageHasSpace = true;
                    }
                }
                else if (rootTarget.TryGetComponent(out Vehicle targetVehicle))
                {
                    switch (targetVehicle)
                    {
                        case Exosuit exosuit:
                        {
                            var container = exosuit.storageContainer.container;
                            targetingStorage = true;
                            if (!container.IsFull())
                            {
                                targetStorageHasSpace = true;
                                targetContainer = container;
                            }

                            break;
                        }
#if SN1
                        case SeaMoth seaMoth:
                        {
                            for (var i = 0; i < 12; i++)
                            {
                                try
                                {
                                    var storage = seaMoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                    if (storage == null) continue;
                                    targetingStorage = true;
                                    if (storage.IsFull()) continue;
                                    targetStorageHasSpace = true;
                                    targetContainer = storage;
                                    break;
                                }
                                catch
                                {
                                    // ignored
                                }
                            }

                            break;
                        }
#endif
                    }
                }
#if SN1
                else
                {
                    EscapePod escapePod = activeTarget.GetComponentInParent<EscapePod>();
                    if (escapePod != null)
                    {
                        var container = escapePod.storageContainer.container;
                        targetingStorage = true;
                        if (!container.IsFull())
                        {
                            targetStorageHasSpace = true;
                            targetContainer = container;
                        }
                    }
                }
#endif

                if (targetContainer != null && GameInput.GetButtonDown(GameInput.Button.AltTool))
                {
                    foreach (ItemsContainer.ItemGroup itemGroup in new List<ItemsContainer.ItemGroup>(
                                 storageContainer.container._items.Values))
                    {
                        foreach (InventoryItem inventoryItem in new List<InventoryItem>(itemGroup.items))
                        {
                            if (targetContainer.IsTechTypeAllowed(inventoryItem.item.overrideTechUsed
                                    ? inventoryItem.item.overrideTechType
                                    : inventoryItem.item.GetTechType()) &&
                                ((IItemsContainer) targetContainer).AddItem(inventoryItem))
                            {
                                ErrorMessage.AddMessage(
                                    $"Moved {inventoryItem.item.GetTechName()} to {targetName}");
                            }
                            else if (storageContainers.Count() > 1)
                            {
                                var newContainer = (storageContainers.Where(container =>
                                    {

                                        if (QModServices.Main.ModPresent("FCSAlterraHub"))
                                        {
                                            Type containerType = container.GetType();
                                            if (containerType.ToString().StartsWith("FCS"))
                                            {
                                                if (CanBeStored is null)
                                                {
                                                    CanBeStored = containerType.GetMethod("CanBeStored",
                                                        BindingFlags.Public | BindingFlags.Instance);
                                                }
                                                else
                                                {
                                                    foreach (ItemsContainer.ItemGroup itemGroup in storageContainer
                                                                 .container._items.Values)
                                                    {
                                                        InventoryItem item = itemGroup.items.Count > 0
                                                            ? itemGroup.items[0]
                                                            : null;
                                                        if (item == null) continue;
                                                        TechType techType = item.item.overrideTechUsed
                                                            ? item.item.overrideTechType
                                                            : item.item.GetTechType();
                                                        if ((bool) CanBeStored.Invoke(container,
                                                                new object[] {1, techType}))
                                                            return ((IItemsContainer) container.container).AddItem(inventoryItem);
                                                    }
                                                }

                                                return false;
                                            }
                                        }
                                        return !container.container.IsFull() &&
                                               container.container.IsTechTypeAllowed(inventoryItem.item.overrideTechUsed
                                                   ? inventoryItem.item.overrideTechType
                                                   : inventoryItem.item.GetTechType()) &&
                                               ((IItemsContainer) container.container).AddItem(inventoryItem);
                                    })
                                    .Select(container => container.container)).FirstOrDefault();
                                if (newContainer != null)
                                {
                                    targetContainer = newContainer;
                                }
                            }
                        }
                    }
                }
            }

            if (!Player.main.IsInside())
            {
                if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
                {
                    if (!__instance.trigger.enabled && !ResetTriggers[__instance])
                    {
                        __instance.trigger.enabled = true;
                        __instance.gameObject.GetComponent<Pickupable>().attached = false;
                    }
                    else
                    {
                        __instance.DeactivatePads();
                        __instance.trigger.enabled = false;
                        ResetTriggers[__instance] = false;
                        __instance.gameObject.GetComponent<Pickupable>().attached = true;
                    }
                }
            }
            else if (__instance.trigger.enabled)
            {
                __instance.DeactivatePads();
                __instance.trigger.enabled = false;
                __instance.gameObject.GetComponent<Pickupable>().attached = true;
            }

            if (!targetingStorage && GameInput.GetButtonDown(GameInput.Button.AltTool))
            {
                storageContainer.Open(__instance.transform);
            }

            string primaryString = "";
            string secondaryString;

            if (Player.main.IsInside())
            {
                secondaryString =
                    $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Open Storage", uGUI.FormatButton(GameInput.Button.AltTool))}";
            }
            else
            {
                string gravtrapactivate =
                    storageContainer.container.IsFull() ? "Cannot Activate, Storage is Full" :
                    !__instance.trigger.enabled && !ResetTriggers[__instance] ? "Activate Gravtrap" :
                    "Deactivate Gravtrap";

                primaryString =
                    $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Deploy Gravtrap", uGUI.FormatButton(GameInput.Button.RightHand))}";
                secondaryString =
                    $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", gravtrapactivate, uGUI.FormatButton(GameInput.Button.LeftHand))}\n{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Open Storage", uGUI.FormatButton(GameInput.Button.AltTool))}";
            }

            if (targetingStorage)
            {
                string transferString = !targetStorageHasSpace
                    ? $"Cannot transfer as {targetName} is full."
                    : $"Transfer Gravtrap contents to {targetName}";

                secondaryString =
                    $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", transferString, uGUI.FormatButton(GameInput.Button.AltTool))}";
            }

#if SN1
            if (!targetingStorage)
                HandReticle.main.interactText1 = primaryString;
            HandReticle.main.interactText2 = secondaryString;
#else
            if(!targetingStorage)
                HandReticle.main.textUse = primaryString;
            HandReticle.main.textUseSubscript = secondaryString;
#endif
        }

        if (__instance.trigger.enabled && storageContainer.container.IsFull())
        {
            __instance.DeactivatePads();
            __instance.trigger.enabled = false;
            __instance.gameObject.GetComponent<Pickupable>().attached = true;
        }

        if (__instance.attractableList.Count < 1 && __instance.trigger.enabled)
        {
            __instance.trigger.enabled = false;
            ResetTriggers[__instance] = true;
            return;
        }

        if (ResetTriggers[__instance])
        {
            __instance.trigger.enabled = true;
            ResetTriggers[__instance] = false;
        }
    }

    [HarmonyPatch(typeof(Gravsphere), nameof(Gravsphere.IsValidTarget))]
    public static void Postfix(Gravsphere __instance, GameObject obj, ref bool __result)
    {
        if (!__result || !StorageContainers.TryGetValue(__instance, out StorageContainer container))
            return;

        if (obj.TryGetComponent(out ResourceTracker tracker) &&
            !obj.TryGetComponent(out ResourceTrackerUpdater updater))
        {
            tracker.StartUpdatePosition();
        }


        if (Vector3.Distance(__instance.transform.position, obj.transform.position) >
            Main.ConfigFile.Distance) return;

        if (obj.TryGetComponent(out BreakableResource resource))
        {
            resource.BreakIntoResources();
            __result = false;
            __instance.trigger.enabled = false;
            ResetTriggers[__instance] = true;
            return;
        }

        if (!obj.TryGetComponent(out Pickupable pickupable)) return;

        TechType techType = pickupable.overrideTechUsed ? pickupable.overrideTechType : pickupable.GetTechType();
        container.container.allowedTech.AddIfNotPresent(techType);
        if (container.container.AddItem(pickupable) == null) return;
        if (tracker != null)
            tracker.OnPickedUp(pickupable);

        PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
        if (Inventory.main.container.Contains(TechType.Scanner) && entryData != null)
        {
            if (!PDAScanner.GetPartialEntryByKey(techType, out PDAScanner.Entry entry))
            {
                entry = PDAScanner.Add(techType, 1);
            }

            if (entry != null)
            {
                PDAScanner.partial.Remove(entry);
                PDAScanner.complete.AddIfNotPresent(entry.techType);
                PDAScanner.NotifyRemove(entry);
                PDAScanner.Unlock(entryData, true, true);
                obj.SendMessage("OnScanned", null, SendMessageOptions.DontRequireReceiver);
#if SN1
                ResourceTracker.UpdateFragments();
#endif
            }
        }

        if (!KnownTech.Contains(techType))
            KnownTech.Add(techType, true);
        GoalManager.main.OnCustomGoalEvent("Pickup_" + techType.AsString(false));
        pickupable.PlayPickupSound();
        pickupable.Deactivate();
        pickupable.transform.SetParent(container.transform);
        __result = false;
    }
}