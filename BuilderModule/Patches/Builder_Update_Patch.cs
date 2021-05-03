namespace BuilderModule.Patches
{
    using HarmonyLib;
    using QModManager.Utility;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    [HarmonyPatch(typeof(Builder), nameof(Builder.Update))]
    internal class Builder_Update_Patch
    {
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
                        codes[i] = new CodeInstruction(OpCodes.Call, typeof(Builder_Update_Patch).GetMethod("VehicleCheck"));
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
            if(Player.main.GetVehicle() == null)
            {
                Builder.inputHandler.canHandleInput = true;
                InputHandlerStack.main.Push(Builder.inputHandler);
            }
            else
            {
                Builder.inputHandler.canHandleInput = false;
            }
        }
    }

}
