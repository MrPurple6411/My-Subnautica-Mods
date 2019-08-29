using System;
using Harmony;

namespace ResourceOverload
{
    [HarmonyPatch(typeof(CellManager))]
    [HarmonyPatch("GetPrefabForSlot", new Type[] { typeof(IEntitySlot)})]
    class IncreaseResourceSpawn
    {

        public static bool Prefix(CellManager __instance, IEntitySlot slot, ref EntitySlot.Filler __result)
        {
            int num = 0;
            bool flag = __instance.spawner == null;
            bool result;
            if (flag)
            {
                __result = default(EntitySlot.Filler);
                result = false;
            }
            else if (slot.IsCreatureSlot())
            {
                do
                {
                    num++;
                    __result = __instance.spawner.GetPrefabForSlot(slot, true);
                }
                while (string.IsNullOrEmpty(__result.classId) && num < 10);
                result = false;
            }
            else
            {
                do
                {
                    num++;
                    __result = __instance.spawner.GetPrefabForSlot(slot, true);
                }
                while (string.IsNullOrEmpty(__result.classId) && num < 100);
                result = false;
            }
            return result;
        }
    }
}
