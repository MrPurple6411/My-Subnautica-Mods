using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LeviathansNeverFlee
{
    public class Main
    {
        public static void Load()
        {
            try
            {
                HarmonyInstance.Create("MrPurple6411.LeviathansNeverFlee").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    [HarmonyPatch(typeof(FleeOnDamage))]
    [HarmonyPatch(nameof(FleeOnDamage.OnTakeDamage))]
    internal class FleeOnDamage_StartPerform_Patch
    {
        [HarmonyPrefix]
        static void Prefix(FleeOnDamage __instance, DamageInfo damageInfo)
        {
            if (__instance.creature.name.ToLower().Contains("leviathan"))
                damageInfo.damage = 0f;
        }
    }
}
