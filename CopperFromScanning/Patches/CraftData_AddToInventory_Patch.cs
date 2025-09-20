namespace CopperFromScanning.Patches;

using System.Linq;
using HarmonyLib;
using CopperFromScanning.Configuration;

[HarmonyPatch(typeof(CraftData), nameof(CraftData.AddToInventory))]
internal class CraftData_AddToInventory_Patch
{
    private static bool ModChecked { get; set; } = false;
    private static bool IsModInstalled { get; set; } = false;

    [HarmonyPrefix]
    private static bool Prefix(TechType techType, int num, bool noMessage, bool spawnIfCantAdd)
    {
        // If user chose to disable this mod's effect, do nothing and let original method run
        if (Main.ModConfig.DisableModEffect)
            return true;

        if (!ModChecked)
        {
            IsModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Any(x => x.Metadata.Name == "IngredientsFromScanning");
            ModChecked = true;
        }

        if (IsModInstalled ||
            techType != TechType.Titanium ||
            num != 2 ||
            noMessage ||
            !spawnIfCantAdd)
            return true;

        if (Main.ModConfig.DisableScanningResourceRewards)
            return false;
        
        CraftData.AddToInventory(TechType.Titanium);
        CraftData.AddToInventory(TechType.Copper);
        return false;
    }
}