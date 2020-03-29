using EasyCraft;
using Harmony;
using QModManager.API.ModLoading;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ChargeRequired
{
    public class Main
    {
		public static bool BatteryCheck(Pickupable pickupable)
		{
			EnergyMixin energyMixin = pickupable.gameObject.GetComponentInChildren<EnergyMixin>();
			if (energyMixin != null)
			{
				GameObject gameObject = energyMixin.GetBattery();
				if (gameObject != null && energyMixin.defaultBattery == CraftData.GetTechType(gameObject))
				{
					IBattery battery = gameObject.GetComponent<IBattery>();
					if (battery.capacity == battery.charge)
						return true;
					else
					{
						return false;
					}
				}
				return false;
			}

			IBattery b2 = pickupable.GetComponent<IBattery>();
			if (b2 != null)
			{
				if (b2.capacity == b2.charge)
					return true;
				else
				{
					return false;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.IsCraftRecipeFulfilled))]
	internal class CrafterLogic_IsCraftRecipeFulfilled
	{
		[HarmonyPostfix]
		public static void CrafterLogic_IsCraftRecipeFulfilled_Postfix(TechType techType, ref bool __result)
		{
			if (__result && GameModeUtils.RequiresIngredients())
			{
				Inventory main = Inventory.main;
#if SUBNAUTICA
				ITechData techData = CraftData.Get(techType, true);
				if (techData != null)
				{
					int i = 0;
					int ingredientCount = techData.ingredientCount;
					while (i < ingredientCount)
					{
						IIngredient ingredient = techData.GetIngredient(i);
#elif BELOWZERO
				IList<Ingredient> ingredients = TechData.GetIngredients(techType);
				if (ingredients != null)
				{
					int i = 0;
					int ingredientCount = ingredients.Count;
					while (i < ingredientCount)
					{
						Ingredient ingredient = ingredients[i];
#endif
						int count = 0;
						IList<InventoryItem> inventoryItems = main.container.GetItems(ingredient.techType);
						if (inventoryItems != null)
						{
							foreach (InventoryItem inventoryItem in inventoryItems)
							{
								if (Main.BatteryCheck(inventoryItem.item))
								{
									count++;
								}
							}
						}
						if (count < ingredient.amount)
						{
							__result = false;
							return;
						}
						i++;
					}
					__result = true;
					return;
				}
				__result = false;
				return;
			}
		}
	}

	[HarmonyPatch(typeof(ClosestItemContainers), nameof(ClosestItemContainers.DestroyItem))]
	internal class ClosestItemContainers_DestroyItem
	{
		[HarmonyPrefix]
		public static bool ClosestItemContainers_DestroyItem_Prefix(TechType techType, ref bool __result, int count = 1)
		{
			int num = 0;
			foreach (ItemsContainer itemsContainer in ClosestItemContainers.containers)
			{
				List<InventoryItem> items = new List<InventoryItem>();
				itemsContainer.GetItems(techType, items);
				foreach (InventoryItem item in items)
				{
					if (Main.BatteryCheck(item.item)) 
						if (itemsContainer.RemoveItem(item.item)) 
							num++;

					if (num == count)
						break;
				}

				if (num == count)
					break;
			}
			if (num < count)
			{
				Console.WriteLine(string.Format("[EasyCraft] Unable to remove {0} {1}", count, techType));
				__result = false;
				return false;
			}

			__result = true;
			Console.WriteLine(string.Format("[EasyCraft] removed {0} {1}", count, techType));
			return false;
		}
	}

	[HarmonyPatch(typeof(ClosestItemContainers), nameof(ClosestItemContainers.GetPickupCount))]
	internal class ClosestItemContainers_GetPickupCount
	{
		[HarmonyPrefix]
		public static bool ClosestItemContainers_GetPickupCount_Prefix(TechType techType, ref int __result)
		{
			int num = 0;
			foreach (ItemsContainer itemsContainer in ClosestItemContainers.containers)
			{
				List<InventoryItem> items = new List<InventoryItem>();
				itemsContainer.GetItems(techType, items);
				foreach (InventoryItem item in items)
				{
					if (Main.BatteryCheck(item.item))
					{
						num++;
					}
				}
			}
			__result = num;
			return false;
		}
	}
}
