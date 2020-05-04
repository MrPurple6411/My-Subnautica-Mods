using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;

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
                yield return code.opcode == OpCodes.Ldstr ? new CodeInstruction(OpCodes.Ldstr, string.Empty) : code;
            }
        }
    }
}