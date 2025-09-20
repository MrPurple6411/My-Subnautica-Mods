namespace UnknownName.Patches;

using HarmonyLib;
using System.Collections;
using UnityEngine;
using UWE;

#if SUBNAUTICA_EXP
[HarmonyPatch(typeof(Inventory), nameof(Inventory.PickupAsync))]
#else
[HarmonyPatch(typeof(Inventory), nameof(Inventory.Pickup))]
#endif
public class Inventory_Pickup
{

    [HarmonyPostfix]
    public static void Postfix(Pickupable pickupable)
	{
		ProcessPickupable(pickupable);
	}

	private static void ProcessPickupable(Pickupable pickupable)
	{
		var techType = pickupable.GetTechType();
		var entryData = PDAScanner.GetEntryData(techType);
		var gameObject = pickupable.gameObject;
		if (Main.Config.ScanOnPickup && Inventory.main.container.Contains(TechType.Scanner) && entryData != null)
		{
			if (!PDAScanner.GetPartialEntryByKey(techType, out var entry))
			{
				entry = PDAScanner.Add(techType, 1);
			}
			if (entry != null)
			{
				PDAScanner.partial.Remove(entry);
				if (!PDAScanner.complete.Contains(techType))
					PDAScanner.complete.Add(entry.techType);
				PDAScanner.NotifyRemove(entry);
				PDAScanner.Unlock(entryData, true, true);
				if (!Main.Config.Hardcore)
					KnownTech.Add(techType, true);
				if (gameObject != null)
				{
					gameObject.SendMessage("OnScanned", null, SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		if (!Main.Config.Hardcore && entryData == null)
		{
			KnownTech.Add(techType, true);
		}
	}

}