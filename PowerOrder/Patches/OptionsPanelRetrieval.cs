namespace PowerOrder.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.Awake))]
    public static class OptionsPanelRetrieval
    {
        [HarmonyPostfix]
        public static void Postfix(uGUI_OptionsPanel __instance)
        {
            Main.optionsPanel = __instance;
        }
    }
}
