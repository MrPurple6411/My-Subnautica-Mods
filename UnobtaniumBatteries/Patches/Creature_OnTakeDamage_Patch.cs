#if SN1
namespace UnobtaniumBatteries.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(CreatureDeath), nameof(CreatureDeath.OnTakeDamage))]
    public static class Creature_OnTakeDamage_Patch
    {

        [HarmonyPostfix]
        public static void Postfix(CreatureDeath __instance)
        {
            if(__instance.TryGetComponent(out WaterParkCreature waterParkCreature) && waterParkCreature.IsInsideWaterPark())
                return;

            if (__instance.liveMixin == null || !Main.typesToMakePickupable.Contains(CraftData.GetTechType(__instance.gameObject))) return;

            if(__instance.liveMixin.IsAlive() && __instance.liveMixin.health <= (__instance.liveMixin.initialHealth / 5))
            {
                var pickupable = __instance.gameObject.EnsureComponent<Pickupable>();
                pickupable.isPickupable = true;
                pickupable.randomizeRotationWhenDropped = true;
            }
            else if(__instance.gameObject.TryGetComponent(out Pickupable pickupable))
            {
                pickupable.isPickupable = false;
            }
        }
    }
}
#endif