namespace Keep_Inventory_On_Death.Patches
{
    using HarmonyLib;

    [HarmonyPatch]
    public static class Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.LoseItems))]
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
