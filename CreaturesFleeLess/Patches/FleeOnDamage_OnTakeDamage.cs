namespace CreaturesFleeLess.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(FleeOnDamage), nameof(FleeOnDamage.OnTakeDamage))]
    internal class FleeOnDamage_OnTakeDamage
    {
        [HarmonyPrefix]
        private static void Prefix(FleeOnDamage __instance, DamageInfo damageInfo)
        {
            if(damageInfo.damage < ((__instance.creature.liveMixin?.health ?? 0) / 2))
            {
                damageInfo.damage = 0f;
            }
        }
    }
}