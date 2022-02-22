namespace Increased_Resource_Spawns.Patches
{
    using HarmonyLib;
    using UWE;

    [HarmonyPatch(typeof(CellManager), nameof(CellManager.GetPrefabForSlot), new[] { typeof(IEntitySlot) })]
    internal class IncreaseResourceSpawn
    {
        public static void Postfix(CellManager __instance, IEntitySlot slot, ref EntitySlot.Filler __result)
        {
            var num = 0;
            if (__instance.spawner == null || slot.IsCreatureSlot() || Main.SmcConfig.ResourceMultiplier <= 1) return;
            while(string.IsNullOrEmpty(__result.classId) && num < Main.SmcConfig.ResourceMultiplier)
            {
                var found = __instance.spawner.GetPrefabForSlot(slot);
                if(!string.IsNullOrEmpty(found.classId) &&
                   WorldEntityDatabase.TryGetInfo(found.classId, out var entityInfo) &&
                   entityInfo.techType != TechType.None &&
                   !Main.SmcConfig.Blacklist.Contains(entityInfo.techType.AsString()))
                {
                    if(Main.SmcConfig.WhiteList.Count == 0 || Main.SmcConfig.WhiteList.Contains(entityInfo.techType.AsString()))
                    {
                        __result = found;
                    }
                }
                num++;
            }
        }
    }
}