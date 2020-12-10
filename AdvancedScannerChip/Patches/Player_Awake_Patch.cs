using AdvancedScannerChip.MonoBehaviours;
using HarmonyLib;

namespace AdvancedScannerChip.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Player_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<ScannerChipFunctionality>();
        }
    }
}
