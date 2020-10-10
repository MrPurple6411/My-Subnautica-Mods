using HarmonyLib;

namespace FabricatorNoAutoClose.Patches
{
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
