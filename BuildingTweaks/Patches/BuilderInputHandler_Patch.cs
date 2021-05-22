namespace BuildingTweaks.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(BuildModeInputHandler), "IInputHandler.HandleLateInput")]
    public static class BuilderInputHandler_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if(!__result)
                return;

            if(Main.Config.AttachToTarget && Builder.placementTarget != null && Builder.canPlace && GameInput.GetButtonHeld(GameInput.Button.LeftHand))
            {
                Freeze_Patches.Freeze = true;
                return;
            }

        }
    }
}
