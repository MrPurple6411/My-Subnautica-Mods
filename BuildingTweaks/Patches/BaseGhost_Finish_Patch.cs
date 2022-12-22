namespace BuildingTweaks.Patches
{
    using BepInEx.Logging;
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using UnityEngine;

    [HarmonyPatch(typeof(BaseGhost), nameof(BaseGhost.Finish))]
    internal class BaseGhost_Finish_Patch
    {
        private static GameObject gameObject;
        private static GameObject parentObject;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var collection = instructions.ToList();
            var codeInstructions = new List<CodeInstruction>(collection);
            var found = false;

            for(var i = 0; i < collection.Count() - 2; i++)
            {
                var currentInstruction = codeInstructions[i];
                var secondInstruction = codeInstructions[i + 1];
                var thirdInstruction = codeInstructions[i + 2];

                if(currentInstruction.opcode == OpCodes.Callvirt
                    && secondInstruction.opcode == OpCodes.Call
                    && thirdInstruction.opcode == OpCodes.Stloc_1)
                {
                    codeInstructions.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_0));
                    codeInstructions.Insert(i + 3, new CodeInstruction(OpCodes.Call, typeof(BaseGhost_Finish_Patch).GetMethod(nameof(CacheObject))));
                    found = true;
                    break;
                }
            }

            if(found is false)
                Main.logSource.Log(LogLevel.Error, $"Cannot find patch location in BaseGhost.Finish");
            else
                Main.logSource.Log(LogLevel.Info, "Transpiler for BaseGhost.Finish completed");

            return codeInstructions.AsEnumerable();
        }

        public static GameObject CacheObject(GameObject builtObject, BaseGhost baseGhost)
        {
            var constructableBase = baseGhost.GetComponentInParent<ConstructableBase>();
            var parent = constructableBase != null ? constructableBase.transform.parent : null;
            parentObject = parent != null ? parent.gameObject : null;
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
