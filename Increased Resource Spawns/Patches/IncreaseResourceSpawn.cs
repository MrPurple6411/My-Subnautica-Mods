namespace Increased_Resource_Spawns.Patches;

using HarmonyLib;
using UWE;

[HarmonyPatch(typeof(CellManager), nameof(CellManager.GetPrefabForSlot), new[] { typeof(IEntitySlot) })]
internal class IncreaseResourceSpawn
{
    public static void Postfix(CellManager __instance, IEntitySlot slot, ref EntitySlot.Filler __result)
    {
        var num = 0;
        if (__instance.spawner == null || slot.IsCreatureSlot() || Main.SMLConfig.ResourceMultiplier <= 1) return;
        while(string.IsNullOrEmpty(__result.classId) && num < Main.SMLConfig.ResourceMultiplier)
        {
            var found = __instance.spawner.GetPrefabForSlot(slot);
            if(!string.IsNullOrEmpty(found.classId) &&
               WorldEntityDatabase.TryGetInfo(found.classId, out var entityInfo) &&
               entityInfo.techType != TechType.None &&
               !Main.SMLConfig.Blacklist.Contains(entityInfo.techType.AsString()))
            {
                if(Main.SMLConfig.WhiteList.Count == 0 || Main.SMLConfig.WhiteList.Contains(entityInfo.techType.AsString()))
                {
                    __result = found;
                }
            }
            num++;
        }
    }
}