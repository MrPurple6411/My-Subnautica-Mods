using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Builder), nameof(Builder.TryPlace))]
    internal class Builder_TryPlace_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            bool found = false;
            bool found2 = false;
            List<OpCode> opCodes = new List<OpCode>() { OpCodes.Ldloc_2, OpCodes.Ldloc_3 };

            for (int i = 0; i < instructions.Count() - 2; i++)
            {
                CodeInstruction currentInstruction = codeInstructions[i];
                CodeInstruction secondInstruction = codeInstructions[i + 1];
                CodeInstruction thirdInstruction = codeInstructions[i + 2];

                if (!found && currentInstruction.opcode == OpCodes.Ldloc_0 && secondInstruction.opcode == OpCodes.Ldc_I4_0 && thirdInstruction.opcode == OpCodes.Ldc_I4_1)
                {

                    codeInstructions.Insert(i + 1, new CodeInstruction(OpCodes.Call, typeof(Builder_TryPlace_Patch).GetMethod(nameof(Builder_TryPlace_Patch.SetBaseParent))));
                    found = true;
                    continue;
                }

                if (opCodes.Contains(currentInstruction.opcode) && secondInstruction.opcode == OpCodes.Callvirt && thirdInstruction.opcode == OpCodes.Dup)
                {
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, typeof(Builder_TryPlace_Patch).GetMethod(nameof(Builder_TryPlace_Patch.SetParent)));
                    found2 = true;
                    break;
                }
                continue;
            }

            if (found is false || found2 is false)
                Logger.Log(Logger.Level.Error, $"Cannot find patch locations {found}:{found2} in Builder.TryPlace");
            else
                Logger.Log(Logger.Level.Info, "Transpiler for Builder.TryPlace completed");

            return codeInstructions.AsEnumerable();
        }

        public static ConstructableBase SetBaseParent(ConstructableBase constructableBase)
        {
            BaseGhost baseGhost = constructableBase.gameObject.GetComponentInChildren<BaseGhost>();
            GameObject placementTarget = Builder.placementTarget;
            if (Main.config.AttachToTarget && baseGhost != null && baseGhost.TargetBase == null && placementTarget != null)
            {
                constructableBase.transform.SetParent(UWE.Utils.GetEntityRoot(placementTarget).transform, true);
            }

            return constructableBase;
        }

        public static Transform SetParent(GameObject builtObject)
        {
            LargeWorldEntity largeWorldEntity = builtObject.GetComponent<LargeWorldEntity>();
            if (largeWorldEntity is null)
            {
                largeWorldEntity = builtObject.AddComponent<LargeWorldEntity>();
                largeWorldEntity.cellLevel = LargeWorldEntity.CellLevel.Medium;
            }
            else if (builtObject.name.Contains("Transmitter"))
            {
                largeWorldEntity.cellLevel = LargeWorldEntity.CellLevel.Global;
            }

            GameObject placementTarget = Builder.placementTarget;
            if (Main.config.AttachToTarget && placementTarget != null)
            {
                builtObject.transform.SetParent(UWE.Utils.GetEntityRoot(placementTarget)?.transform ?? placementTarget.transform);
            }

            return builtObject.transform;
        }
    }

}
