using Harmony;
using QModManager.API.ModLoading;
using System.Reflection;

namespace CopperFromScanning
{
    [HarmonyPatch(typeof(CraftData), nameof(CraftData.AddToInventory))]
    internal class CraftData_AddToInventory
    {
        [HarmonyPrefix]
        private static bool Prefix(TechType techType, int num = 1, bool noMessage = false, bool spawnIfCantAdd = true)
        {
            if(techType == TechType.Titanium && num == 2 && noMessage == false && spawnIfCantAdd == true)
            {
                CraftData.AddToInventory(TechType.Titanium);
                CraftData.AddToInventory(TechType.Copper);
                return false;
            }
            return true;
        }
    }
}