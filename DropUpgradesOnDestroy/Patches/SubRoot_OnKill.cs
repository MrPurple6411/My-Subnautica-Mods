namespace DropUpgradesOnDestroy.Patches;

using HarmonyLib;
using System.Linq;

[HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnKill))]
public class SubRoot_OnKill
{
    [HarmonyPrefix]
    public static void Prefix(SubRoot __instance)
    {
		var equipment = (from UpgradeConsole console in __instance.GetComponentsInChildren<UpgradeConsole>(true)
										 from InventoryItem item in console?.modules?.equipment?.Values?.Where((e) => e != null)
										 select item).ToList();

		var position = __instance.gameObject.transform.position;
        Main.SpawnModuleNearby(equipment, position);
    }
}