namespace PowerOrder.Patches;

using HarmonyLib;
using PowerOrder.Configuration;

[HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.Awake))]
public static class OptionsPanelRetrieval
{
    [HarmonyPostfix]
    public static void Postfix(uGUI_OptionsPanel __instance)
    {
        Options.optionsPanel = __instance;
    }
}
