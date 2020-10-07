using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace BuilderModule.Patches
{

    [HarmonyPatch(typeof(Builder), "Update")]
    internal class Builder_Update_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int codepoint = -1;

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction currentCode = codes[i];
                if (codepoint == -1)
                {
                    if (currentCode.opcode == OpCodes.Ldsfld && currentCode.operand == Builder.inputHandler)
                    {
                        codepoint = i;
                        codes[i] = new CodeInstruction(OpCodes.Call, typeof(Builder_Update_Patch).GetMethod("VehicleCheck"));
                    }
                }
                else if (i == (codepoint + 1) || i == (codepoint + 2) || i == (codepoint + 3) || i == (codepoint + 4) || i == (codepoint + 5))
                {
                    codes[i] = new CodeInstruction(OpCodes.Nop);
                }
            }
            return codes.AsEnumerable();
        }

        public static void VehicleCheck()
        {
            if (Player.main.GetVehicle() == null)
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
