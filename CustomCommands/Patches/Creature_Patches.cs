using HarmonyLib;

namespace CustomCommands.Patches
{
    [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
    class Creature_Start_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Creature __instance)
        {
            TechType techType = CraftData.GetTechType(__instance.gameObject);

            if (Main.CreatureParameters.ContainsKey(techType))
            {
                Pickupable pickupable = __instance.gameObject.GetComponent<Pickupable>();
                __instance.gameObject.EnsureComponent<WaterParkCreature>().pickupable = pickupable;
            }
        }
    }
}
