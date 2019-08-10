using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace MrPurple
{


    [HarmonyPatch(typeof(CraftData))]
    [HarmonyPatch("GetUseEatSound")]
    internal class SoundBreaker
    {

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction code in instructions)
            {
                if (code.opcode == OpCodes.Ldstr)
                    yield return new CodeInstruction(OpCodes.Ldstr, string.Empty);
                else
                    yield return code;
            }
        }



    }
}
