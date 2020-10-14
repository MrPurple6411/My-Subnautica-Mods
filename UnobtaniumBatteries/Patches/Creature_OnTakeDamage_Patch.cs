using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnobtaniumBatteries.Patches
{
    [HarmonyPatch(typeof(Creature), nameof(Creature.OnTakeDamage))]
    public static class Creature_OnTakeDamage_Patch
    {
        public static List<Type> typesToMakePickupable = new List<Type>() { typeof(ReaperLeviathan), typeof(GhostLeviathan), typeof(Warper), typeof(GhostLeviatanVoid) };

        [HarmonyPostfix]
        public static void Postfix(Creature __instance)
        {
            if (typesToMakePickupable.Contains(__instance.GetType()))
            {
                if (__instance.liveMixin.health < (__instance.liveMixin.initialHealth))
                {
                    Pickupable pickupable = __instance.gameObject.EnsureComponent<Pickupable>();
                    pickupable.isPickupable = true;
                    pickupable.randomizeRotationWhenDropped = true;
                }
                else if (__instance.gameObject.TryGetComponent(out Pickupable pickupable))
                {
                    GameObject.Destroy(pickupable);
                }
            }
        }
    }
}
