#if BELOWZERO
namespace DropUpgradesOnDestroy.Patches;

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

[HarmonyPatch(typeof(SeaTruckSegment), nameof(SeaTruckSegment.OnKill))]
public class SeaTruckSegment_OnKill
{
	private static SeaTruckSegment _lastDestroyed;

	[HarmonyPrefix]
	public static void Prefix(SeaTruckSegment __instance)
	{
		if (!__instance.IsFront() || __instance == _lastDestroyed) return;
			_lastDestroyed = __instance;
		var equipment = __instance.motor?.upgrades?.modules?.equipment?.Values.Where((e) => e != null).ToList() ?? new List<InventoryItem>();

		var position = __instance.gameObject.transform.position;
		Main.SpawnModuleNearby(equipment, position);
	}
}
#endif