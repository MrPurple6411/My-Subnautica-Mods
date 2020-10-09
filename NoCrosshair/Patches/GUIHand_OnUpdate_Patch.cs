using HarmonyLib;

namespace NoCrosshair.Patches
{
    [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
    public static class GUIHand_OnUpdate_Patch
    {
        public static void Postfix(GUIHand __instance)
        {
            if (GameInput.GetButtonHeld(GameInput.Button.AltTool) && GameInput.GetButtonDown(GameInput.Button.LeftHand))
            {
                NoCrosshair.check = !NoCrosshair.check;
            }

            if (HandReticle.main.iconType == HandReticle.IconType.Default)
            {
                if ((__instance.GetActiveTarget() == null || Player.main.IsPiloting()) && NoCrosshair.check)
                {
                    NoCrosshair.ChangeCrosshair(false);
                }
                else
                {
                    NoCrosshair.ChangeCrosshair(true);
                }
            }
        }
    }
}