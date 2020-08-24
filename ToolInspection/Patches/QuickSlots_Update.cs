using HarmonyLib;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UWE;

namespace ToolInspection.Patches
{
    [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.Update))]
    public static class QuickSlots_Update
    {
        static float timeCheck = 0;
        static int slot;

        [HarmonyPrefix]
        public static void Prefix(QuickSlots __instance)
        {

            InventoryItem item = __instance.heldItem;
            if(Input.GetKeyDown(KeyCode.I) && item != null && timeCheck == 0)
            {
                TechType techType = item.item.GetTechType();
                PlayerTool tool = item.item?.gameObject?.GetComponent<PlayerTool>();
                FieldInfo firstUseAnimationStarted = AccessTools.Field(typeof(PlayerTool), "firstUseAnimationStarted");
                if (!GameOptions.GetVrAnimationMode() && tool != null && tool.hasFirstUseAnimation)
                {
                    if (Player.main.usedTools.Contains(techType))
                    {
                        Player.main.usedTools.Remove(techType);
                    }

                    slot = __instance.GetSlotByItem(item);
                    __instance.SelectImmediate(slot);
                    timeCheck = Time.time + tool.holsterTime;
                    CoroutineHost.StartCoroutine(SelectDelay(__instance));
                }
            }
        }

        private static IEnumerator SelectDelay(QuickSlots quickSlots)
        {
            while(Time.time < timeCheck)
            {
                yield return new WaitForSeconds(0.01f);
            }

            quickSlots.Select(slot);
            timeCheck = 0;

            yield break;

        }
    }
}
