using HarmonyLib;

namespace NoCrosshair.Patches
{
    [HarmonyPatch(typeof(GUIHand), "OnUpdate")]
    public static class GUIHand_OnUpdate_Patch
    {
        public static void Postfix(GUIHand __instance)
        {
            if (GameInput.GetButtonHeld(GameInput.Button.AltTool) && GameInput.GetButtonDown(GameInput.Button.LeftHand))
            {
                NoCrosshair.check = !NoCrosshair.check;
            }

            HandReticle.IconType iconType = (HandReticle.IconType)AccessTools.Field(typeof(HandReticle), "iconType").GetValue(HandReticle.main);

            if (iconType == HandReticle.IconType.Default)
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