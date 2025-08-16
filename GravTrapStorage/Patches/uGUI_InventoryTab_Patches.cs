namespace GravTrapStorage.Patches;

using HarmonyLib;
using System.Collections;
using System.Text;
using UnityEngine;

[HarmonyPatch]
internal class UGUIInventoryTabPatches
{
	public static InventoryItem CurrentItem { get; set; }

	[HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnPointerEnter))]
	[HarmonyPostfix]
	public static void OnPointerEnter_Postfix(InventoryItem item)
	{
		CurrentItem = item;
	}

	[HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnPointerExit))]
	[HarmonyPostfix]
	public static void OnPointerExit_Postfix()
	{
		CurrentItem = null;
	}

	[HarmonyPatch(typeof(Player), nameof(Player.Update))]
	[HarmonyPostfix]
	public static void Update_Postfix(Player __instance)
	{
		PDA pda = Player.main.GetPDA();
		if (!pda.isInUse) return;
	// Determine controller state with API differences across games
#if BELOWZERO
	var device = GameInput.GetPrimaryDevice();
#else
	var device = GameInput.PrimaryDevice;
#endif
	bool usingController = device == GameInput.Device.Controller;
		if (!GameInput.GetButtonDown(usingController ? GameInput.Button.Sprint : GameInput.Button.AltTool)) return;
		Pickupable pickupable;
		if (CurrentItem != null)
		{
			pickupable = CurrentItem.item;
		}
		else
		{
			GamepadInputModule inputModule = GamepadInputModule.current;
			if (inputModule.currentNavigableGrid?.GetSelectedItem() is not uGUI_ItemIcon { manager: uGUI_ItemsContainer itemsContainer } itemIcon) return;
			if (!itemsContainer.icons.TryGetValue(itemIcon, out InventoryItem inventoryItem)) return;

			pickupable = inventoryItem.item;
		}

		StorageContainer storageContainer = pickupable.gameObject.GetComponentInChildren<StorageContainer>(true);
		if (storageContainer == null) return;

		__instance.StartCoroutine(OpenStorage(pda, pickupable, storageContainer));
	}

	private static IEnumerator OpenStorage(PDA pda, Pickupable pickupable, StorageContainer storageContainer)
	{
		if (!storageContainer.GetOpen())
		{
			pda.Close();
			if (pickupable.gameObject.TryGetComponent(out Gravsphere gravsphere))
				storageContainer.container.SetAllowedTechTypes(new[] { TechType.None });
			storageContainer.Open(storageContainer.transform);
			Inventory.main.SetUsedStorage(storageContainer.container, false);
			yield return new WaitUntil(() => pda.Open(PDATab.Inventory, storageContainer.transform, new PDA.OnClose(storageContainer.OnClosePDA)));
		}
		else
		{
			pda.Close();
			yield return new WaitUntil(()=> pda.Open(PDATab.Inventory));
		}
	}

	[HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemActions))]
	[HarmonyPostfix]
	public static void ItemActions_Postfix(StringBuilder sb, InventoryItem item)
	{
		StorageContainer storageContainer = item.item.gameObject.GetComponentInChildren<StorageContainer>(true);
		if (storageContainer == null) return;

		string msg = storageContainer.GetOpen() ? "Close Storage" : "Open Storage";
	// Determine bindings using current device
#if BELOWZERO
	var device = GameInput.GetPrimaryDevice();
#else
	var device = GameInput.PrimaryDevice;
#endif
	bool usingController = device == GameInput.Device.Controller;
	var bind = GameInput.GetBinding(device, usingController ? GameInput.Button.Sprint : GameInput.Button.AltTool, GameInput.BindingSet.Primary);
	TooltipFactory.WriteAction(sb, bind, msg);
	}
}