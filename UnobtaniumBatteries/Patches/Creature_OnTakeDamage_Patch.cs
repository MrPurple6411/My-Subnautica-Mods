using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnobtaniumBatteries.Patches
{
    [HarmonyPatch(typeof(Creature), nameof(Creature.OnTakeDamage))]
    public static class Creature_OnTakeDamage_Patch
    {

        [HarmonyPostfix]
        public static void Postfix(Creature __instance)
        {
            if (__instance.TryGetComponent(out WaterParkCreature waterParkCreature) && waterParkCreature.IsInsideWaterPark())
                return;

            if (Main.typesToMakePickupable.Contains(__instance.GetType()) && __instance.liveMixin != null)
            {
                if (__instance.liveMixin.health <= (__instance.liveMixin.initialHealth/10) && __instance.liveMixin.IsAlive())
                {
                    Pickupable pickupable = __instance.gameObject.EnsureComponent<Pickupable>();
                    pickupable.isPickupable = true;
                    pickupable.randomizeRotationWhenDropped = true;
                }
                else if (__instance.gameObject.TryGetComponent(out Pickupable pickupable))
                {
                    pickupable.isPickupable = false;
                }
            }
        }
    }
}
