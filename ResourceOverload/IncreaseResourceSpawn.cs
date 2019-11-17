using System;
using Harmony;
using UWE;

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
            bool result = true;
            if (flag)
            {
                __result = default(EntitySlot.Filler);
                result = false;
            }
            else if (!slot.IsCreatureSlot() && Config.ToggleValue)
            {
                while (string.IsNullOrEmpty(__result.classId) && num < Config.SliderValue)
                {
                    num++;
                    __result = __instance.spawner.GetPrefabForSlot(slot, true);
                };
                result = false;
            }
            return result;
        }
    }
}
