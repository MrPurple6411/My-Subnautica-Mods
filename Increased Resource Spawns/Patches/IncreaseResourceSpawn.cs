using System;
using HarmonyLib;

namespace Increased_Resource_Spawns.Patches
{
    [HarmonyPatch(typeof(CellManager))]
    [HarmonyPatch(nameof(CellManager.GetPrefabForSlot), new Type[] { typeof(IEntitySlot) })]
    internal class IncreaseResourceSpawn
    {
        public static void Postfix(CellManager __instance, IEntitySlot slot, ref EntitySlot.Filler __result)
        {
            int num = 0;
            if (__instance.spawner != null && !slot.IsCreatureSlot() && Main.config.ResourceMultiplier > 1)
            {
                while (string.IsNullOrEmpty(__result.classId) && (float)num < Main.config.ResourceMultiplier)
                {
                    num++;
                    __result = __instance.spawner.GetPrefabForSlot(slot, true);
                }
            }
        }
    }
}