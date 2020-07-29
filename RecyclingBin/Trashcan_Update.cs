using System.Collections.Generic;
using HarmonyLib;
using QModManager.API;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace RecyclingBin
{
    [HarmonyPatch(typeof(Trashcan), nameof(Trashcan.Update))]
    internal class Trashcan_Update
    {
        public static List<InventoryItem> inventoryItems;
        public static List<Pickupable> forcePickupItems;

        [HarmonyPrefix]
        public static bool Prefix(Trashcan __instance)
        {
            if (__instance.biohazard)
            {
                return true;
            }

            __instance.storageContainer.hoverText = "Recycling Bin";
            __instance.storageContainer.storageLabel = "Recycling Bin";
            __instance.storageContainer.container._label = "Recycling Bin";

            inventoryItems = new List<InventoryItem>();
            forcePickupItems = new List<Pickupable>();

            foreach (Trashcan.Waste waste in __instance.wasteList)
            {
                InventoryItem item = waste.inventoryItem;

                if (item is null)
                {
                    continue;
                }

                TechType techType = item.item.GetTechType();
#if SUBNAUTICA
                TechData techData = CraftDataHandler.GetTechData(techType);
#elif BELOWZERO
                RecipeData techData = CraftDataHandler.GetRecipeData(techType);
#endif

                if (!GameInput.GetButtonHeld(GameInput.Button.Deconstruct) && techType != TechType.Titanium && techData != null && techData.ingredientCount > 0 && Main.BatteryCheck(item.item))
                {
                    if (CheckRequirements(__instance, item.item, techData))
                    {
                        foreach (Ingredient ingredient in techData.Ingredients)
                        {
                            for (int i = ingredient.amount; i > 0; i--)
                            {
                                GameObject gameObject = CraftData.InstantiateFromPrefab(ingredient.techType, false);
                                gameObject.SetActive(true);
                                Pickupable pickupable = gameObject.GetComponent<Pickupable>();
                                pickupable.Pickup(false);
                                if ((item.item.GetComponent<IBattery>() == null && pickupable.GetComponent<IBattery>() != null && QModServices.Main.ModPresent("NoBattery")) || pickupable.GetComponent<LiveMixin>() != null)
                                {
                                    UnityEngine.Object.Destroy(pickupable.gameObject);
                                }
                                else
                                {
                                    forcePickupItems.Add(pickupable);
                                }
                            }
                        }
                        break;
                    }
                }
                else
                {
                    if (GameInput.GetButtonHeld(GameInput.Button.Deconstruct))
                    {
                        inventoryItems.Add(item);
                    }
                    else
                    {
                        forcePickupItems.Add(item.item);
                    }

                    break;
                }
            }
            forcePickupItems.ForEach((rejectedItem) => Inventory.main.ForcePickup(rejectedItem));
            inventoryItems.ForEach((item) => UnityEngine.Object.Destroy(item.item.gameObject));

            return false;
        }

#if SUBNAUTICA

        private static bool CheckRequirements(Trashcan __instance, Pickupable item, TechData techData)
#elif BELOWZERO

        private static bool CheckRequirements(Trashcan __instance, Pickupable item, RecipeData techData)
#endif
        {
            bool check = true;
            int craftCountNeeded = techData.craftAmount;
            IList<InventoryItem> inventoryItems = __instance.storageContainer.container.GetItems(item.GetTechType());
            if (inventoryItems != null && inventoryItems.Count >= craftCountNeeded)
            {
                while (craftCountNeeded > 0)
                {
                    Trashcan_Update.inventoryItems.Add(inventoryItems[craftCountNeeded - 1]);
                    craftCountNeeded--;
                }

                foreach (TechType techType in techData.LinkedItems)
                {
                    int linkedCountNeeded = techData.LinkedItems.FindAll((TechType tt) => tt == techType).Count;
                    IList<InventoryItem> inventoryItems2 = __instance.storageContainer.container.GetItems(techType);
                    IList<InventoryItem> inventoryItems3 = Inventory.main.container.GetItems(techType);
                    int count = (inventoryItems2?.Count ?? 0) + (inventoryItems3?.Count ?? 0);
                    if (count < linkedCountNeeded)
                    {
                        ErrorMessage.AddMessage($"Missing {linkedCountNeeded - (inventoryItems2?.Count + inventoryItems3?.Count)} {techType.ToString()}");
                        Inventory.main.ForcePickup(item);
                        Trashcan_Update.inventoryItems.Clear();
                        return false;
                    }

                    int count1 = inventoryItems2?.Count ?? 0;
                    int count2 = inventoryItems3?.Count ?? 0;
                    while (linkedCountNeeded > 0)
                    {
                        if (count1 > 0)
                        {
                            Trashcan_Update.inventoryItems.Add(inventoryItems2[count1 - 1]);
                            count1--;
                        }
                        else if (count2 > 0)
                        {
                            Trashcan_Update.inventoryItems.Add(inventoryItems3[count2 - 1]);
                            count2--;
                        }
                        linkedCountNeeded--;
                    }
                }
            }
            else
            {
                check = false;
            }

            return check;
        }
    }
}