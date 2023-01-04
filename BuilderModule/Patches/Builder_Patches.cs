namespace BuilderModule.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Reflection.Emit;
#if BZ
    using Module;
    using UnityEngine;
#endif

    [HarmonyPatch]
    internal class Builder_Patches
    {
        [HarmonyPatch(typeof(Builder), nameof(Builder.Update))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            var matcher = new CodeMatcher(instructions)
                .MatchForward(true, new CodeMatch((currentCode) =>currentCode.opcode == OpCodes.Ldsfld && currentCode.operand.ToString().Contains("inputHandler")));

            if(!matcher.IsValid)
            {
                Main.logSource.LogError("Builder Update Transpiler injection point NOT found!!  Game has most likely updated and broken this mod!");
                return instructions;
            }

            matcher.SetAndAdvance(OpCodes.Call, typeof(Builder_Patches).GetMethod("VehicleCheck"));
            matcher.RemoveInstructions(5);
            return matcher.InstructionEnumeration();
        }

        public static void VehicleCheck()
        {
            if(Player.main.GetVehicle() != null
#if BZ
                || Player.main.IsPilotingSeatruck() || Player.main.inHovercraft
#endif
                )
            {
                Builder.inputHandler.canHandleInput = false;
                return;
            }

            Builder.inputHandler.canHandleInput = true;
            InputHandlerStack.main.Push(Builder.inputHandler);
        }

#if BZ
        [HarmonyPatch(typeof(Builder), nameof(Builder.GetAimTransform))]
        [HarmonyPostfix]
        public static void Postfix(ref Transform __result)
        {
            if(Main.btConfig != null)
            {
                var attach = (bool)Main.AttachToTargetField.GetValue(Main.btConfig);
                if(!attach)
                    return;
            }
            var builderModule = Player.main.GetComponentInParent<BuilderModuleMono>();
            if (builderModule == null || !builderModule.isToggle) return;
            var lights = builderModule.gameObject.GetComponentsInChildren<Light>()?? new Light[0];
            foreach(var light in lights)
            {
                if (light.gameObject.name != "light_center") continue;
                __result = light.gameObject.transform;
                return;
            }
        }
#endif
    }

}
