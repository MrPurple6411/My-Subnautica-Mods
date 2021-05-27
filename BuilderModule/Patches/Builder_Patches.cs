namespace BuilderModule.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using Logger = QModManager.Utility.Logger;
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
            var codepoint = -1;

            var codeInstructions = instructions.ToList();
            var codes = new List<CodeInstruction>(codeInstructions);
            for(var i = 0; i < codeInstructions.Count(); i++)
            {
                var currentCode = codes[i];
                if(codepoint == -1)
                {
                    if (currentCode.opcode != OpCodes.Ldsfld ||
                        !currentCode.operand.ToString().Contains("inputHandler")) continue;
                    codepoint = i;
                    codes[i] = new CodeInstruction(OpCodes.Call, typeof(Builder_Patches).GetMethod("VehicleCheck"));
                }
                else if(i == (codepoint + 1) || i == (codepoint + 2) || i == (codepoint + 3) || i == (codepoint + 4) || i == (codepoint + 5))
                {
                    codes[i] = new CodeInstruction(OpCodes.Nop);
                }
            }

            if(codepoint > -1)
                Logger.Log(Logger.Level.Debug, $"Builder Update Transpiler Found and Patched.");
            else
                throw new System.Exception("Builder Update Transpiler injection point NOT found!!  Game has most likely updated and broken this mod!");

            return codes.AsEnumerable();
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
