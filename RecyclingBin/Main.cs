using Harmony;
using QModManager.API;
using QModManager.API.ModLoading;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RecyclingBin
{
	[QModCore]
	public class Main
	{
		[QModPatch]
		public void Load()
		{
			try
			{
				HarmonyInstance.Create("MrPurple6411.RecyclingBin").PatchAll(Assembly.GetExecutingAssembly());


				LanguageHandler.SetTechTypeName(TechType.Trashcans, "Recycling Bin");
				LanguageHandler.SetTechTypeTooltip(TechType.Trashcans, "Breaks items down to the most basic materials. \nNote: Batteries and Tools must be fully charged to be recycled.");
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static TechData GetData(Pickupable item)
		{
			try
			{
				return CraftDataHandler.GetTechData(item.GetTechType());
			}
			catch (Exception)
			{
				try
				{
					ITechData td = CraftData.Get(item.GetTechType());
					TechData techData = new TechData() { craftAmount = td.craftAmount };
					for (int i = 0; i < td.ingredientCount; i++)
					{
						IIngredient ingredient = td.GetIngredient(i);
						Ingredient smlingredient = new Ingredient(ingredient.techType, ingredient.amount);
						techData.Ingredients.Add(smlingredient);
					}
					for (int i = 0; i < td.linkedItemCount; i++)
					{
						techData.LinkedItems.Add(td.GetLinkedItem(i));
					}
					return techData;

				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		public static bool BatteryCheck(Pickupable pickupable)
		{
			EnergyMixin energyMixin = pickupable.gameObject.GetComponentInChildren<EnergyMixin>();
			if (energyMixin != null)
			{
				GameObject gameObject = energyMixin.GetBattery(); 
				bool defaultCheck = false;
				if (gameObject != null) defaultCheck = energyMixin.defaultBattery == CraftData.GetTechType(gameObject);

				if (gameObject == null && QModServices.Main.ModPresent("NoBattery"))
				{
					return true;
				}
				if (gameObject != null && (defaultCheck || QModServices.Main.ModPresent("NoBattery")))
				{
					IBattery battery = gameObject.GetComponent<IBattery>();

					TechData techData = GetData(pickupable);
					bool recipeCheck = techData.Ingredients.FindAll((ingredient) => ingredient.techType == TechType.Battery || ingredient.techType == TechType.PrecursorIonBattery || ingredient.techType == TechType.LithiumIonBattery || ingredient.techType == TechType.PowerCell || ingredient.techType == TechType.PrecursorIonPowerCell).Count == 0;
					if (battery != null && QModServices.Main.ModPresent("NoBattery") && recipeCheck)
					{
						ErrorMessage.AddMessage($"{pickupable.GetTechType().ToString()} has a battery in it. Cannot Recycle.");
						return false;
					}
					else if (battery != null && defaultCheck && battery.charge > (battery.capacity * 0.99))
					{
						return true;
					}
					else
					{
						if (gameObject != null && !defaultCheck)
							ErrorMessage.AddMessage($"{CraftData.GetTechType(gameObject).ToString()} is not the default battery for {pickupable.GetTechType().ToString()}.");
						else
							ErrorMessage.AddMessage($"{pickupable.GetTechType().ToString()} is not fully charged and cannot be recycled.");
						return false;
					}
				}
				else
				{
					if (gameObject != null)
						ErrorMessage.AddMessage($"{CraftData.GetTechType(gameObject).ToString()} is not the default battery for {pickupable.GetTechType().ToString()}.");
					else
						ErrorMessage.AddMessage($"{pickupable.GetTechType().ToString()} has no battery.");

					return false;
				}
			}

			IBattery b2 = pickupable.GetComponent<IBattery>();
			if (b2 != null)
			{
				if (b2.charge > (b2.capacity * 0.99))
					return true;
				else
				{
					ErrorMessage.AddMessage($"{pickupable.GetTechType().ToString()} is not fully charged and cannot be recycled.");
					return false;
				}
			}
			return true;
		}

	}

	[HarmonyPatch(typeof(Trashcan), nameof(Trashcan.IsAllowedToAdd))]
	internal class Trashcan_IsAllowedToAdd
	{
		[HarmonyPrefix]
		public static bool Prefix(ref bool __result)
		{
			__result = true;
			return false;
		}
	}

	[HarmonyPatch(typeof(Trashcan), nameof(Trashcan.Update))]
	internal class Trashcan_Update
	{
		public static List<InventoryItem> inventoryItems;
		public static List<Pickupable> forcePickupItems;

		[HarmonyPrefix]
		public static bool Prefix(Trashcan __instance)
		{
			__instance.storageContainer.hoverText = "Recycling Bin";
			__instance.storageContainer.storageLabel = "Recycling Bin";
			__instance.storageContainer.container._label = "Recycling Bin";

			inventoryItems = new List<InventoryItem>();
			forcePickupItems = new List<Pickupable>();

			foreach (Trashcan.Waste waste in __instance.wasteList)
			{
				InventoryItem item = waste.inventoryItem;
				TechData techData = Main.GetData(item.item);

				if (item.item.GetTechType() != TechType.Titanium && Main.BatteryCheck(item.item) && techData != null)
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
									UnityEngine.Object.Destroy(pickupable.gameObject);
								else
									forcePickupItems.Add(pickupable);
							}
						}
						break;
					}
				}
				else
				{
					forcePickupItems.Add(item.item);
					break;
				}
			}
			forcePickupItems.ForEach((rejectedItem) => Inventory.main.ForcePickup(rejectedItem));
			inventoryItems.ForEach((item) => UnityEngine.Object.Destroy(item.item.gameObject));

			return false;
		}

		private static bool CheckRequirements(Trashcan __instance, Pickupable item, TechData techData)
		{
			bool check = true;
			int craftCountNeeded = techData.craftAmount;
			IList<InventoryItem> inventoryItems = __instance.storageContainer.container.GetItems(item.GetTechType());
			if (inventoryItems != null && inventoryItems.Count >= craftCountNeeded)
			{
				while (craftCountNeeded > 0)
				{
					Trashcan_Update.inventoryItems.Add(inventoryItems[craftCountNeeded-1]);
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

					int count1 = (inventoryItems2?.Count ?? 0);
					int count2 = (inventoryItems3?.Count ?? 0);
					while (linkedCountNeeded > 0)
					{
						if (count1 > 0)
						{
							Trashcan_Update.inventoryItems.Add(inventoryItems2[count1-1]);
							count1--;
						}
						else if(count2 > 0)
						{
							Trashcan_Update.inventoryItems.Add(inventoryItems3[count2-1]);
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
