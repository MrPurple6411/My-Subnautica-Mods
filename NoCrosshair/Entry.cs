using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace NoCrosshair
{
    public static class NoCrosshair
    {
        private static bool mapCheck;
        private static bool check = true;
        private static uGUI_HandReticleIcon icon;
        private static Dictionary<HandReticle.IconType, uGUI_HandReticleIcon> icons;

        private static void ChangeCrosshair(bool show)
        {
            if (!show)
            {
                if (!icon)
                {
                    icon = icons[HandReticle.IconType.Default];
                }
                icons.Remove(HandReticle.IconType.Default);
            }
            else if (icon)
            {
                icons[HandReticle.IconType.Default] = icon;
            }
            icon.SetActive(show, 0.1f);
        }

        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.Awake))]
        private static class HandReticle_Awake
        {
            private static void Postfix(HandReticle __instance)
            {
                icons = __instance._icons;
                ChangeCrosshair(false);
            }
        }

        [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
        private static class GUIHand_OnUpdate
        {
            private static void Postfix(GUIHand __instance)
            {
                if (GameInput.GetButtonHeld(GameInput.Button.AltTool) && GameInput.GetButtonDown(GameInput.Button.LeftHand))
                    check = !check;

                if(HandReticle.main.iconType == HandReticle.IconType.Default)
                {
                    if ((__instance.GetActiveTarget() == null || Player.main.IsPiloting()) && check)
                        ChangeCrosshair(false);
                    else
                        ChangeCrosshair(true);
                }
            }
        }

        [HarmonyPatch(typeof(uGUI_MapRoomScanner), nameof(uGUI_MapRoomScanner.OnTriggerEnter))]
        private static class MapRoomScanner_OnTriggerEnter
        {
            private static void Postfix()
            {
                if (check)
                {
                    mapCheck = check;
                    check = false;
                }
            }
        }

        [HarmonyPatch(typeof(uGUI_MapRoomScanner), nameof(uGUI_MapRoomScanner.OnTriggerExit))]
        private static class MapRoomScanner_OnTriggerExit
        {
            private static void Postfix()
            {
                if (mapCheck)
                {
                    check = mapCheck;
                    mapCheck = false;
                }
            }
        }

    }
}
