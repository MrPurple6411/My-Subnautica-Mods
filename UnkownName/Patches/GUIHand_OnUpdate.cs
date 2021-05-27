namespace UnKnownName.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
    public class GUIHand_OnUpdate
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            var scanTarget = PDAScanner.scanTarget;
            var entryData = PDAScanner.GetEntryData(scanTarget.techType);
            var key = entryData?.key ?? TechType.None;
            var blueprint = entryData?.blueprint ?? TechType.None;

            if(scanTarget.techType != TechType.None && (CrafterLogic.IsCraftRecipeUnlocked(scanTarget.techType) || (entryData != null && ((blueprint != TechType.None && CrafterLogic.IsCraftRecipeUnlocked(entryData.blueprint)) || (key != TechType.None && CrafterLogic.IsCraftRecipeUnlocked(entryData.key)))) || !scanTarget.isValid || !GameModeUtils.RequiresBlueprints()))
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