using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerOrder.Patches
{
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
