using System;
using HarmonyLib;
using UWE;

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
                while (string.IsNullOrEmpty(__result.classId) && num < Main.config.ResourceMultiplier)
                {
                    EntitySlot.Filler found = __instance.spawner.GetPrefabForSlot(slot, true);
                    if (!string.IsNullOrEmpty(found.classId) && 
                        WorldEntityDatabase.TryGetInfo(found.classId, out WorldEntityInfo entityInfo) && 
                        entityInfo.techType != TechType.None && 
                        !Main.config.Blacklist.Contains(entityInfo.techType.AsString()))
                    {
                        if(Main.config.WhiteList.Count == 0 || Main.config.WhiteList.Contains(entityInfo.techType.AsString()))
                        {
                            __result = found;
                        }
                    }
                    num++;
                }
            }
        }
    }
}