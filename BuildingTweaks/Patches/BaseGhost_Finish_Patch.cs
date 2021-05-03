namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;

    [HarmonyPatch(typeof(BaseGhost), nameof(BaseGhost.Finish))]
    internal class BaseGhost_Finish_Patch
    {
        public static GameObject gameObject;
        public static GameObject parentObject;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            bool found = false;

            for(int i = 0; i < instructions.Count() - 2; i++)
            {
                CodeInstruction currentInstruction = codeInstructions[i];
                CodeInstruction secondInstruction = codeInstructions[i + 1];
                CodeInstruction thirdInstruction = codeInstructions[i + 2];

                if(currentInstruction.opcode == OpCodes.Callvirt
                    && secondInstruction.opcode == OpCodes.Call
#if SUBNAUTICA_STABLE
                    && thirdInstruction.opcode == OpCodes.Stloc_2)
#else
                    && thirdInstruction.opcode == OpCodes.Stloc_1)
#endif
                {
                    codeInstructions.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_0));
                    codeInstructions.Insert(i + 3, new CodeInstruction(OpCodes.Call, typeof(BaseGhost_Finish_Patch).GetMethod(nameof(BaseGhost_Finish_Patch.CacheObject))));
                    found = true;
                    break;
                }
                continue;
            }

            if(found is false)
                Logger.Log(Logger.Level.Error, $"Cannot find patch location in BaseGhost.Finish");
            else
                Logger.Log(Logger.Level.Info, "Transpiler for BaseGhost.Finish completed");

            return codeInstructions.AsEnumerable();
        }

        public static GameObject CacheObject(GameObject builtObject, BaseGhost baseGhost)
        {
            parentObject = baseGhost.GetComponentInParent<ConstructableBase>()?.transform?.parent?.gameObject;
            gameObject = builtObject;
            return builtObject;
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            if(gameObject != null && parentObject != null)
            {
                gameObject.transform.SetParent(parentObject.transform);
            }

            gameObject = null;
            parentObject = null;
        }
    }

}
