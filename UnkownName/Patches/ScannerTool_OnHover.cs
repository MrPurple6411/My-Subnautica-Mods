namespace UnknownName.Patches;

using HarmonyLib;

[HarmonyPatch(typeof(ScannerTool), nameof(ScannerTool.OnHover))]
public class ScannerTool_OnHover
{
    [HarmonyPostfix]
    public static void Postfix(ScannerTool __instance)
    {
        var scanTarget = PDAScanner.scanTarget;
        var result = PDAScanner.CanScan(scanTarget);
        var entryData = PDAScanner.GetEntryData(PDAScanner.scanTarget.techType);

        if((entryData != null && (CrafterLogic.IsCraftRecipeUnlocked(entryData.blueprint) || CrafterLogic.IsCraftRecipeUnlocked(entryData.key))) || PDAScanner.ContainsCompleteEntry(scanTarget.techType) || __instance.energyMixin.charge <= 0f || !scanTarget.isValid || result != PDAScanner.Result.Scan ||
#if SUBNAUTICA
			!GameModeUtils.RequiresBlueprints()
#else
			!GameModeManager.GetOption<bool>(GameOption.TechRequiresUnlocking)
#endif
			)
		{
            return;
        }
        HandReticle.main.SetText(HandReticle.TextType.Hand, Main.Config.UnKnownLabel, true);
    }

}