using HarmonyLib;

namespace CustomCommands.Patches
{
#if BZ
    [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.Start))]
    class WaterParkCreature_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(WaterParkCreature __instance)
        {
            TechType techType = CraftData.GetTechType(__instance.gameObject);

            if (Main.CreatureParameters.ContainsKey(techType))
            {
               __instance.data = Main.CreatureParameters[techType];
            }
        }
    }
#endif
}
