using Harmony;
using System;
using System.Reflection;
using UnityEngine;

namespace LeviathansNeverFlee
{
    [HarmonyPatch(typeof(FleeOnDamage))]
    [HarmonyPatch(nameof(FleeOnDamage.OnTakeDamage))]
    internal class FleeOnDamage_StartPerform_Patch
    {
        [HarmonyPrefix]
        static void Prefix(FleeOnDamage __instance, DamageInfo damageInfo)
        {
            if (__instance.creature.name.ToLower().Contains("leviathan"))
            {
                damageInfo.damage = 0f;
            }
        }
    }
}
