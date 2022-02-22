namespace CopperFromScanning.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(CraftData), nameof(CraftData.AddToInventory))]
    internal class CraftData_AddToInventory_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(TechType techType, int num, bool noMessage, bool spawnIfCantAdd)
        {
            if (SMCLib.API.ModServices.ModPresent("IngredientsFromScanning") ||
                techType != TechType.Titanium ||
                num != 2 ||
                noMessage ||
                !spawnIfCantAdd)
                return true;
            
            CraftData.AddToInventory(TechType.Titanium);
            CraftData.AddToInventory(TechType.Copper);
            return false;
        }
    }
}