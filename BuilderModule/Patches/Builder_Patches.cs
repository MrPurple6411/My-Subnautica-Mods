namespace BuilderModule.Patches
{
    using BuilderModule.Module;
    using HarmonyLib;
    using QModManager.Utility;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;

    [HarmonyPatch]
    internal class Builder_Patches
    {
        [HarmonyPatch(typeof(Builder), nameof(Builder.Update))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int codepoint = -1;

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for(int i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction currentCode = codes[i];
                if(codepoint == -1)
                {
                    if(currentCode.opcode == OpCodes.Ldsfld && currentCode.operand.ToString().Contains("inputHandler"))
                    {
                        codepoint = i;
                        codes[i] = new CodeInstruction(OpCodes.Call, typeof(Builder_Patches).GetMethod("VehicleCheck"));
                    }
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
                bool attach = (bool)Main.AttachToTargetField.GetValue(Main.btConfig);
                if(!attach)
                    return;
            }
            BuilderModuleMono builderModule = Player.main.GetComponentInParent<BuilderModuleMono>();
            if(builderModule != null && builderModule.isToggle)
            {
                Light[] lights = builderModule.gameObject.GetComponentsInChildren<Light>()?? new Light[0];
                foreach(Light light in lights)
                {
                    if(light.gameObject.name == "light_center")
                    {
                        __result = light.gameObject.transform;
                        return;
                    }
                }
            }
            return;
        }
#endif
    }

}
