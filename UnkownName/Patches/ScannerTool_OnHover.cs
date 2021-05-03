namespace UnKnownName.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(ScannerTool), nameof(ScannerTool.OnHover))]
    public class ScannerTool_OnHover
    {
        [HarmonyPostfix]
        public static void Postfix(ScannerTool __instance)
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
#if SN1
            PDAScanner.Result result = PDAScanner.CanScan();
#elif BZ
            PDAScanner.Result result = PDAScanner.CanScan(scanTarget);
#endif
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(PDAScanner.scanTarget.techType);

            if((entryData != null && (CrafterLogic.IsCraftRecipeUnlocked(entryData.blueprint) || CrafterLogic.IsCraftRecipeUnlocked(entryData.key))) || PDAScanner.ContainsCompleteEntry(scanTarget.techType) || __instance.energyMixin.charge <= 0f || !scanTarget.isValid || result != PDAScanner.Result.Scan || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }
#if SN1
            HandReticle.main.SetInteractText(Main.Config.UnKnownLabel, false, HandReticle.Hand.None);
#elif BZ
            HandReticle.main.SetText(HandReticle.TextType.Hand, Main.Config.UnKnownLabel, true, GameInput.Button.None);
#endif
        }

    }

}