namespace CreaturesFleeLess.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(FleeOnDamage), nameof(FleeOnDamage.OnTakeDamage))]
    internal class FleeOnDamage_OnTakeDamage
    {
        [HarmonyPrefix]
        private static void Prefix(FleeOnDamage __instance, DamageInfo damageInfo)
        {
            var liveMixin = __instance.creature.liveMixin;
            if(liveMixin != null && damageInfo.damage < liveMixin.health / 2)
            {
                damageInfo.damage = 0f;
            }
        }
    }
}