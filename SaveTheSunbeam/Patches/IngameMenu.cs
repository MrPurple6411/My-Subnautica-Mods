using Harmony;

namespace SaveTheSunbeam.Patches
{
    [HarmonyPatch(typeof(IngameMenu))]
    [HarmonyPatch("GetAllowSaving")]
    static class IngameMenu_GetAllowSaving
    {
        static void Postfix(ref bool __result)
        {
            __result = __result && !Mod.disableSaving;
        }
    }
}
