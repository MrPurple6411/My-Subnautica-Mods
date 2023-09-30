#pragma warning disable 618
namespace BuilderModule.Patches;

using HarmonyLib;
using System.Collections;
using UnityEngine;
using UWE;

[HarmonyPatch(typeof(Constructable), nameof(Constructable.Construct))]
internal class Construct_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(Constructable __instance)
    {
        var player = Player.main;
        if(player.isPiloting && GameModeUtils.RequiresIngredients())
        {
            if(__instance._constructed)
                return false;
#if BELOWZERO
            if(player.GetComponentInParent<Hoverbike>() is not null)
                return true;
#endif

            var count = __instance.resourceMap.Count;
            var resourceID = __instance.GetResourceID();
            var backupConstructedAmount = __instance.constructedAmount;
            __instance.constructedAmount += Time.deltaTime / (count * Constructable.GetConstructInterval());
            __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
            var resourceID2 = __instance.GetResourceID();
            if(resourceID2 != resourceID)
            {
                var destroyTechType = __instance.resourceMap[resourceID2 - 1];
                var thisVehicle = player.GetVehicle();
                var storageCheck = false;
                if(thisVehicle != null)
                {
                    switch(thisVehicle)
                    {
                        case Exosuit exosuit:
                            if (exosuit.storageContainer.container.Contains(destroyTechType) &&
                                exosuit.storageContainer.container.DestroyItem(destroyTechType))
                                storageCheck = true;
                            break;

                        case SeaMoth seaMoth:
                            for(var i = 0; i < 12; i++)
                            {
                                try
                                {
                                    var storage = seaMoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                    if (storage == null || !storage.Contains(destroyTechType)) continue;
                                    if (!storage.DestroyItem(destroyTechType)) continue;
                                    storageCheck = true;
                                    break;
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                            break;
                    }
                    if(!storageCheck)
                    {
                        __instance.constructedAmount = backupConstructedAmount;
                        return true;
                    }
                }
#if BELOWZERO
                var seaTruck = player.GetComponentInParent<SeaTruckUpgrades>();
                if(seaTruck != null)
                {
                    foreach(var storageContainer in seaTruck.GetComponentsInChildren<StorageContainer>() ?? new StorageContainer[0])
                    {
                        try
                        {
                            if (!storageContainer.container.DestroyItem(destroyTechType)) continue;
                            storageCheck = true;
                            break;
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                if(!storageCheck)
                {
                    __instance.constructedAmount = backupConstructedAmount;
                    return true;
                }
#endif
            }
            __instance.UpdateMaterial();
            if(__instance.constructedAmount >= 1f)
            {
                _ = __instance.SetState(true);
            }
            return false;
        }
        else
        {
            return true;
        }
    }
}

[HarmonyPatch(typeof(Constructable), nameof(Constructable.DeconstructAsync))]
internal class Deconstruct_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(Constructable __instance, IOut<bool> result)
    {
        var player = Player.main;
        if(player.isPiloting && GameModeUtils.RequiresIngredients())
        {
            if(__instance._constructed)
                return true;
#if BELOWZERO
            if(player.GetComponentInParent<Hoverbike>() is not null)
                return true;
#endif
            var count = __instance.resourceMap.Count;

            var resourceID = __instance.GetResourceID();
            var backupConstructedAmount = __instance.constructedAmount;
            __instance.constructedAmount -= Time.deltaTime / (count * Constructable.GetConstructInterval());
            __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
            var resourceID2 = __instance.GetResourceID();
            if(resourceID2 != resourceID)
            {
                var techType = __instance.resourceMap[resourceID2];

                var size =
#if SUBNAUTICA
                    CraftData.GetItemSize(techType);
#elif BELOWZERO
                    TechData.GetItemSize(techType);
#endif

                var storageCheck = false;
                var thisVehicle = Player.main.GetVehicle();
                if(thisVehicle != null)
                {
                    switch (thisVehicle)
                    {
                        case Exosuit exosuit:
                        {
                            var storageContainer = exosuit.storageContainer;

                            if(storageContainer.container.HasRoomFor(size.x, size.y))
                            {
                                CoroutineHost.StartCoroutine(AddToVehicle(techType, storageContainer.container));
                                storageCheck = true;
                            }
                            break;
                        }
                        case SeaMoth seaMoth:
                        {
                            for(var i = 0; i < 12; i++)
                            {
                                try
                                {
                                    var storage = seaMoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                    if (storage == null || !storage.HasRoomFor(size.x, size.y)) continue;
                                    CoroutineHost.StartCoroutine(AddToVehicle(techType, storage));
                                    storageCheck = true;
                                    break;
                                }
                                catch
                                {
                                    // ignored
                                }
                            }

                            break;
                        }
                    }
                }
#if BELOWZERO
                var seaTruck = player.GetComponentInParent<SeaTruckUpgrades>();
                if(seaTruck != null)
                {
                    foreach(var storageContainer in seaTruck.GetComponentsInChildren<StorageContainer>() ?? new StorageContainer[0])
                    {
                        try
                        {
                            var storage = storageContainer.container;
                            if (storage == null || !storage.HasRoomFor(size.x, size.y)) continue;
                            CoroutineHost.StartCoroutine(AddToVehicle(techType, storage));
                            storageCheck = true;
                            break;
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
#endif

                if(!storageCheck)
                {
                    __instance.constructedAmount = backupConstructedAmount;
                    return true;
                }
            }
            __instance.UpdateMaterial();

            result.Set(__instance.constructedAmount <= 0f);
            return false;
        }
        return true;
    }

    private static IEnumerator AddToVehicle(TechType techType, ItemsContainer itemsContainer)
    {
        var coroutineTask = CraftData.GetPrefabForTechTypeAsync(techType, false);
        yield return coroutineTask;

        var prefab = coroutineTask.GetResult() ?? global::Utils.CreateGenericLoot(techType);

        var gameObject = Object.Instantiate(prefab, null);
        var pickupable = gameObject.EnsureComponent<Pickupable>();

        pickupable.Initialize();
        var item = new InventoryItem(pickupable);
        itemsContainer.UnsafeAdd(item);
        var name = Language.main.GetOrFallback(techType.AsString(), techType.AsString());
        ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));
        uGUI_IconNotifier.main.Play(techType, uGUI_IconNotifier.AnimationType.From);
        pickupable.PlayPickupSound();
    }
}
