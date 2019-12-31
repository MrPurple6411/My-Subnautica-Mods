using System;
using Harmony;
using WorldStreaming;

namespace AmpeelBallsNoMore
{
    public static class Entry
    {
        public static void Patch()
        {
            HarmonyInstance.Create("MrPurple6411.AmpeelBallsNoMore").PatchAll();
        }
    }

    [HarmonyPatch(typeof(FleeOnDamage))]
    [HarmonyPatch(nameof(FleeOnDamage.OnTakeDamage))]
    internal class FleeOnDamage_StartPerform_Patch
    {
        [HarmonyPrefix]
        static void Prefix(FleeOnDamage __instance, DamageInfo damageInfo)
        {
            if (__instance.creature.name.ToLower().Contains("shocker") && damageInfo.type == DamageType.Electrical)
                damageInfo.damage = 0f;
        }
    }
}