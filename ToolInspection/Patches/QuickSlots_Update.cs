namespace ToolInspection.Patches
{
    using HarmonyLib;
    using System.Collections;
    using UnityEngine;
    using UWE;

    [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.UpdateState))]
    internal class QuickSlots_UpdateState
    {
        private static float timeCheck;

        [HarmonyPrefix]
        private static void Prefix(QuickSlots __instance)
        {
            if (!Input.GetKeyDown(KeyCode.I)|| uGUI_PDA.main.tabOpen != PDATab.None || DevConsole.instance.state || timeCheck != 0) return;
            var item = __instance.heldItem;
            var techType = item?.item?.GetTechType() ?? TechType.None;
            var tool = item?.item?.gameObject.GetComponent<PlayerTool>();
            if (GameOptions.GetVrAnimationMode() || tool == null || !tool.hasFirstUseAnimation) return;
            if(Player.main.usedTools.Contains(techType))
                Player.main.usedTools.Remove(techType);

            var slot = __instance.GetSlotByItem(item);
            if (slot == -1) return;
            __instance.SelectImmediate(slot);
            timeCheck = Time.time + tool.holsterTime;
            CoroutineHost.StartCoroutine(SelectDelay(__instance, slot));
        }

        private static IEnumerator SelectDelay(QuickSlots quickSlots, int slot)
        {
            while(Time.time < timeCheck)
            {
                yield return new WaitForSeconds(0.01f);
            }

            quickSlots.Select(slot);
            timeCheck = 0;
        }
    }
}
