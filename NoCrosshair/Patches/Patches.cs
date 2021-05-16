namespace NoCrosshair.Patches
{
    using HarmonyLib;

    [HarmonyPatch]
    public static class Patches
    {
        internal static bool check = true;
        internal static bool mapCheck;

        [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
        [HarmonyPostfix]
        public static void OnUpdate_Postfix(GUIHand __instance)
        {
            if(GameInput.GetButtonHeld(GameInput.Button.AltTool) && GameInput.GetButtonDown(GameInput.Button.LeftHand))
                check = !check;

            if(HandReticle.main.iconType == HandReticle.IconType.Default && HandReticle.main._icons.TryGetValue(HandReticle.IconType.Default, out uGUI_HandReticleIcon icon))
                icon.SetActive(__instance.GetActiveTarget() != null && (!Player.main.IsPiloting() || !check), 0.1f);
        }

        [HarmonyPatch(typeof(uGUI_MapRoomScanner), nameof(uGUI_MapRoomScanner.OnTriggerEnter))]
        [HarmonyPostfix]
        public static void OnTriggerEnter_Postfix()
        {
            if(check)
            {
                mapCheck = check;
                check = false;
            }
        }

        [HarmonyPatch(typeof(uGUI_MapRoomScanner), nameof(uGUI_MapRoomScanner.OnTriggerExit))]
        [HarmonyPostfix]
        public static void OnTriggerExit_Postfix()
        {
            if(mapCheck)
            {
                check = mapCheck;
                mapCheck = false;
            }
        }
    }
}