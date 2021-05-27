namespace UnKnownName.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(ScannerTool), nameof(ScannerTool.OnHover))]
    public class ScannerTool_OnHover
    {
        [HarmonyPostfix]
        public static void Postfix(ScannerTool __instance)
        {
            var scanTarget = PDAScanner.scanTarget;
#if SN1
            var result = PDAScanner.CanScan();
#elif BZ
            var result = PDAScanner.CanScan(scanTarget);
#endif
            var entryData = PDAScanner.GetEntryData(PDAScanner.scanTarget.techType);

            if((entryData != null && (CrafterLogic.IsCraftRecipeUnlocked(entryData.blueprint) || CrafterLogic.IsCraftRecipeUnlocked(entryData.key))) || PDAScanner.ContainsCompleteEntry(scanTarget.techType) || __instance.energyMixin.charge <= 0f || !scanTarget.isValid || result != PDAScanner.Result.Scan || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }
#if SN1
            HandReticle.main.SetInteractText(Main.Config.UnKnownLabel, false);
#elif BZ
            HandReticle.main.SetText(HandReticle.TextType.Hand, Main.Config.UnKnownLabel, true);
#endif
        }

    }

}