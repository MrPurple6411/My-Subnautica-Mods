namespace ToolInspection.Patches;

using HarmonyLib;
using System.Collections;
using UnityEngine;
using UWE;

[HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.UpdateState))]
internal class QuickSlots_UpdateState
{
    private static float _timeCheck;

    [HarmonyPrefix]
    private static void Prefix(QuickSlots __instance)
    {
        if (__instance._heldItem is null ||
#if SUBNAUTICA
			GameOptions.GetVrAnimationMode() ||
#endif
			!Input.GetKeyDown(KeyCode.I) || (DevConsole.instance?.state ?? true) || uGUI_PDA.main.tabOpen != PDATab.None || _timeCheck != 0) return;

        var item = __instance.heldItem;
        var techType = item.item.GetTechType();
        if(techType == TechType.None) return;
        if (!item.item.gameObject.TryGetComponent(out PlayerTool tool) || !tool.hasFirstUseAnimation) return;

        if(Player.main.usedTools.Contains(techType))
            Player.main.usedTools.Remove(techType);

        var slot = __instance.GetSlotByItem(item);
        if (slot == -1) return;
        __instance.SelectImmediate(slot);
        _timeCheck = Time.time + tool.holsterTime;
        CoroutineHost.StartCoroutine(SelectDelay(__instance, slot));
    }

    private static IEnumerator SelectDelay(QuickSlots quickSlots, int slot)
    {
        while(Time.time < _timeCheck)
        {
            yield return new WaitForSeconds(0.01f);
        }

        quickSlots.Select(slot);
        _timeCheck = 0;
    }
}
