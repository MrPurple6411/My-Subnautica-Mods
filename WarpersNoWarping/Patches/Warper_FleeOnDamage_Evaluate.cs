namespace WarpersNoWarping.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(FleeOnDamage), nameof(FleeOnDamage.Evaluate))]
    public class Warper_FleeOnDamage_Evaluate
    {
        [HarmonyPostfix]
        public static void Postfix(Creature creature, ref float __result)
        {
            if(creature.GetType() == typeof(Warper))
            {
                __result = 0f;
            }
        }
    }
}