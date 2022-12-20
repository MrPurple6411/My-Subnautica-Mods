namespace CopperFromScanning.Patches
{
    using System.Linq;
    using HarmonyLib;

    [HarmonyPatch(typeof(CraftData), nameof(CraftData.AddToInventory))]
    internal class CraftData_AddToInventory_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(TechType techType, int num, bool noMessage, bool spawnIfCantAdd)
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Any(x => x.Metadata.Name == "IngredientsFromScanning") ||
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