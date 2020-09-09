using HarmonyLib;

namespace CustomCommands.Patches
{
#if BZ
    [HarmonyPatch(typeof(WaterParkCreature), "Start")]
    class WaterParkCreature_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(WaterParkCreature __instance)
        {
            TechType techType = CraftData.GetTechType(__instance.gameObject);

            if (Main.CreatureParameters.ContainsKey(techType))
            {
                AccessTools.Field(typeof(WaterParkCreature),"data").SetValue(__instance, Main.CreatureParameters[techType]);
            }
        }
    }
#endif
}
