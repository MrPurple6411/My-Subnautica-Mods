namespace FabricatorNoAutoClose.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
    public static class Crafter_OnCraftingBegin_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}
