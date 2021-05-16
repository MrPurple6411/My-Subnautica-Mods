namespace BuildingTweaks.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HarmonyLib;

    [HarmonyPatch(typeof(BuildModeInputHandler), "IInputHandler.HandleLateInput")]
    public static class BuilderInputHandler_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if(!__result)
                return;

            if(Builder.placementTarget != null && Builder.canPlace && GameInput.GetButtonHeld(GameInput.Button.AltTool))
            {
                    __result = !Builder.TryPlace();
                return;
            }
        }
    }
}
