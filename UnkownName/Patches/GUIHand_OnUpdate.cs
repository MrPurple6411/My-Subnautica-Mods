using HarmonyLib;

namespace UnKnownName.Patches
{
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

            if (scanTarget.techType != TechType.None && KnownTech.Contains(scanTarget.techType) || (entryData != null && ((blueprint != TechType.None && KnownTech.Contains(entryData.blueprint)) || (key != TechType.None && KnownTech.Contains(entryData.key)))) || !scanTarget.isValid || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }
#if SUBNAUTICA
            HandReticle.main.SetInteractText(Main.config.UnKnownLabel, false, HandReticle.Hand.None);
#elif BELOWZERO
            HandReticle.main.SetText(HandReticle.TextType.Hand, Main.config.UnKnownLabel, true, GameInput.Button.None);
#endif
        }
    }

}