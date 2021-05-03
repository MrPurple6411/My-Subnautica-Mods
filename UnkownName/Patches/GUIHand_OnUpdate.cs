namespace UnKnownName.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
    public class GUIHand_OnUpdate
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(scanTarget.techType);
            TechType key = entryData?.key ?? TechType.None;
            TechType blueprint = entryData?.blueprint ?? TechType.None;

            if(scanTarget.techType != TechType.None && (CrafterLogic.IsCraftRecipeUnlocked(scanTarget.techType) || (entryData != null && ((blueprint != TechType.None && CrafterLogic.IsCraftRecipeUnlocked(entryData.blueprint)) || (key != TechType.None && CrafterLogic.IsCraftRecipeUnlocked(entryData.key)))) || !scanTarget.isValid || !GameModeUtils.RequiresBlueprints()))
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