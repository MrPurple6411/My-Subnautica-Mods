using HarmonyLib;

namespace WarpersNoWarping
{
    [HarmonyPatch(typeof(Warper), nameof(Warper.OnKill))]
    public class Warper_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(Warper __instance)
        {
            __instance.WarpOut();
        }
    }

    [HarmonyPatch(typeof(WarperInspectPlayer), nameof(WarperInspectPlayer.Perform))]
    public class WarperInspectPlayer_Perform
    {
        [HarmonyPrefix]
        public static void Prefix(WarperInspectPlayer __instance)
        {
            __instance.warpOutDistance = 0;
        }
    }

    [HarmonyPatch(typeof(WarpOut), nameof(WarpOut.Evaluate))]
    public class WarpOut_Evaluate
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result)
        {
            __result = 0f;
            return false;
        }
    }

    [HarmonyPatch(typeof(FleeOnDamage), nameof(FleeOnDamage.Evaluate))]
    public class Warper_FleeOnDamage_Evaluate
    {
        [HarmonyPostfix]
        public static void Postfix(Creature creature, ref float __result)
        {
            if (creature.GetType() == typeof(Warper))
            {
                __result = 0f;
            }
        }
    }
}