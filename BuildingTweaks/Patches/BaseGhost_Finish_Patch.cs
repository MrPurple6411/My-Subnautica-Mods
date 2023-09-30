namespace BuildingTweaks.Patches;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

[HarmonyPatch]
internal class BaseGhost_Finish_Patch
{
    private static GameObject gameObject;
    private static GameObject parentObject;


    [HarmonyPatch(typeof(BaseGhost), nameof(BaseGhost.Finish))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions)
            .MatchForward(true,
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Stloc_1));

        if(!matcher.IsValid)
        {
            Main.logSource.LogError("Cannot find patch location in BaseGhost.Finish");
            return instructions;
        }

        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, typeof(BaseGhost_Finish_Patch).GetMethod(nameof(CacheObject))));
        return matcher.InstructionEnumeration();
    }

    public static GameObject CacheObject(GameObject builtObject, BaseGhost baseGhost)
    {
        var constructableBase = baseGhost.GetComponentInParent<ConstructableBase>();
        var parent = constructableBase != null ? constructableBase.transform.parent : null;
        parentObject = parent != null ? parent.gameObject : null;
        gameObject = builtObject;
        return builtObject;
    }

    [HarmonyPatch(typeof(BaseGhost), nameof(BaseGhost.Finish))]
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
