using Harmony;
using System;
using UWE;

namespace ResourceOverload
{
    [HarmonyPatch(typeof(CellManager))]
    [HarmonyPatch(nameof(CellManager.GetPrefabForSlot), new Type[] { typeof(IEntitySlot) })]
    internal class IncreaseResourceSpawn
    {
        [HarmonyPostfix]
        public static void Postfix(CellManager __instance, IEntitySlot slot, ref EntitySlot.Filler __result)
        {
            if(!string.IsNullOrEmpty(__result.classId))
            {
                return;
            }

            int num = 1;
            bool flag = __instance.spawner == null;
            if(flag)
            {
                __result = default;
            }
            else if(!slot.IsCreatureSlot())
            {
                while(string.IsNullOrEmpty(__result.classId) && num < Config.ResourceMultiplier)
                {
                    num++;
                    __result = __instance.spawner.GetPrefabForSlot(slot, true);
                    if(!string.IsNullOrEmpty(__result.classId))
                    {
                        WorldEntityInfo wei;
                        if(WorldEntityDatabase.TryGetInfo(__result.classId, out wei))
                        {
                            if(wei.techType == TechType.TimeCapsule)
                            {
                                ErrorMessage.AddWarning("!!!!!Time Capsule Spawned!!!!!");
                            }
                        }
                    }
                };
            }
        }
    }
}