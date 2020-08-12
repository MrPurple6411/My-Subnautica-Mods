using HarmonyLib;

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(LanguageCache), nameof(LanguageCache.GetPickupText))]
    public class LanguageCache_GetPickupText
    {
        [HarmonyPostfix]
        public static void Postfix(ref string __result, TechType techType)
        {
            if (!KnownTech.Contains(techType) && GameModeUtils.RequiresBlueprints())
            {
                __result = Main.config.UnKnownLabel;
            }
        }
    }

}