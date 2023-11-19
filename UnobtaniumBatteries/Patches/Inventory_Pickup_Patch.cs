#if SUBNAUTICA
namespace UnobtaniumBatteries.Patches;

using HarmonyLib;
using UnityEngine;

#if SUBNAUTICA
[HarmonyPatch(typeof(Inventory), nameof(Inventory.Pickup))]
#else
[HarmonyPatch(typeof(Inventory), nameof(Inventory.PickupAsync))]
#endif
public static class Inventory_Pickup_Patch
{
	[HarmonyPostfix]
	public static void Postfix(Pickupable pickupable)
	{
		if (pickupable.GetTechType() != TechType.Warper) return;
		var warper = pickupable.GetComponent<Warper>();
		if (warper == null || warper.spawner == null) return;

		warper.spawner.warper = null;
		GameObject.DestroyImmediate(warper.spawner);
		warper.spawner = null;
	}
}
#endif