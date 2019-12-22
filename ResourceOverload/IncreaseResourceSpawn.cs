using System;
using System.Collections.Generic;
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
                __result = default;
                result = false;
            }
            else if (!slot.IsCreatureSlot() && Config.ToggleValue)
            {
                while (string.IsNullOrEmpty(__result.classId) && num < Config.SliderValue)
                {
                    num++;
                    __result = __instance.spawner.GetPrefabForSlot(slot, true);
                    if (!string.IsNullOrEmpty(__result.classId))
                    {
                        WorldEntityInfo wei;
                        if (WorldEntityDatabase.TryGetInfo(__result.classId, out wei))
                        {
                            if (wei.techType == TechType.TimeCapsule)
                            {
                                ErrorMessage.AddWarning("!!!!!Time Capsule Spawned!!!!!");
                            }
                        }
                    }
                };
                result = false;
            }
            else if (slot.IsCreatureSlot() && Config.ToggleValue)
            {
                __result = __instance.spawner.GetPrefabForSlot(slot, true);
                WorldEntityInfo wei;
                if (!string.IsNullOrEmpty(__result.classId))
                {
                    if (WorldEntityDatabase.TryGetInfo(__result.classId, out wei))
                    {
                        if (wei.techType == TechType.ReaperLeviathan)
                        {
                            ErrorMessage.AddWarning("!!!!!WARNING Reaper Leviathan WARNING!!!!!");
                        }
                    }
                }
                result = false;
            }
            return result;
        }
    }
}
