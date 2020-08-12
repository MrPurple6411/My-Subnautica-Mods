using HarmonyLib;

namespace CopperFromScanning.Patches
{
    [HarmonyPatch(typeof(CraftData), nameof(CraftData.AddToInventory))]
    internal class CraftData_AddToInventory_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(TechType techType, int num = 1, bool noMessage = false, bool spawnIfCantAdd = true)
        {
            if (techType == TechType.Titanium && num == 2 && noMessage == false && spawnIfCantAdd == true)
            {
                CraftData.AddToInventory(TechType.Titanium);
                CraftData.AddToInventory(TechType.Copper);
                return false;
            }
            return true;
        }
    }
}