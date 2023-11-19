namespace BuildingTweaks.Patches;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

[HarmonyPatch(typeof(Builder), nameof(Builder.TryPlace))]
internal class Builder_TryPlace_Patch
{

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Ldc_I4_1));

        if(matcher.IsInvalid)
        {
            Main.Logger.LogError($"Cannot find patch location 1 in Builder.TryPlace");
            return instructions;
        }

        matcher.Advance(1);
        matcher.Insert(new CodeInstruction(OpCodes.Call, typeof(Builder_TryPlace_Patch).GetMethod(nameof(SetBaseParent))));

        var opCodes = new List<OpCode>() { OpCodes.Ldloc_2, OpCodes.Ldloc_3 };
        matcher.MatchForward(false,
                new CodeMatch((currentInstruction)=> opCodes.Contains(currentInstruction.opcode)),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Dup));

        if(matcher.IsInvalid)
        {
            Main.Logger.LogError($"Cannot find patch location 2 in Builder.TryPlace");
            return instructions;
        }

        matcher.Advance(1);
        matcher.Set(OpCodes.Call, typeof(Builder_TryPlace_Patch).GetMethod(nameof(SetParent)));
        return matcher.InstructionEnumeration();
    }

    public static ConstructableBase SetBaseParent(ConstructableBase constructableBase)
    {
        var baseGhost = constructableBase.gameObject.GetComponentInChildren<BaseGhost>();

        if(Main.Config.AttachToTarget && baseGhost != null && baseGhost.TargetBase == null && Builder.placementTarget != null)
        {
            var placementTarget = UWE.Utils.GetEntityRoot(Builder.placementTarget) ?? Builder.placementTarget;
            if(placementTarget.TryGetComponent(out LargeWorldEntity largeWorldEntity))
            {
                largeWorldEntity.cellLevel = LargeWorldEntity.CellLevel.Global;
                largeWorldEntity.initialCellLevel = LargeWorldEntity.CellLevel.Global;

                LargeWorldStreamer.main.cellManager.RegisterEntity(largeWorldEntity);
            }

            if(constructableBase.gameObject.TryGetComponent(out Collider builtCollider))
            {
                foreach(var collider in placementTarget.GetComponentsInChildren<Collider>() ?? new Collider[0])
                {
                    Physics.IgnoreCollision(collider, builtCollider);
                }
            }

            constructableBase.transform.SetParent(placementTarget.transform);
        }

        return constructableBase;
    }

    public static Transform SetParent(GameObject builtObject)
    {
        var largeWorldEntity = builtObject.GetComponent<LargeWorldEntity>();
        if(largeWorldEntity is null)
        {
            largeWorldEntity = builtObject.AddComponent<LargeWorldEntity>();
            largeWorldEntity.cellLevel = LargeWorldEntity.CellLevel.Medium;
        }
        else if(builtObject.name.Contains("Transmitter"))
        {
            largeWorldEntity.cellLevel = LargeWorldEntity.CellLevel.Global;
            largeWorldEntity.initialCellLevel = LargeWorldEntity.CellLevel.Global;
        }

        if(Main.Config.AttachToTarget && Builder_Update_Patches.Freeze && Builder.ghostModel.transform.parent is null)
        {
            var aimTransform = Builder.GetAimTransform();
            if(Physics.Raycast(aimTransform.position, aimTransform.forward, out var hit, Builder.placeMaxDistance, Builder.placeLayerMask.value, QueryTriggerInteraction.Ignore))
            {
                var hitCollider = hit.collider;
                Builder.placementTarget = hitCollider.gameObject;
            }
        }

        if(Main.Config.AttachToTarget || (Builder.placementTarget is not null && builtObject.GetComponent<ConstructableBase>() is null))
        {
            var placementTarget = Builder.placementTarget is not null ? UWE.Utils.GetEntityRoot(Builder.placementTarget) ?? Builder.placementTarget : null;
            GameObject finalTarget = GetFinalTarget(placementTarget);

            if(finalTarget != null)
            {
                foreach(var builtCollider in builtObject.GetComponentsInChildren<Collider>(true) ?? Array.Empty<Collider>())
                {
                    foreach(var collider in finalTarget.GetComponentsInChildren<Collider>(true) ?? Array.Empty<Collider>())
                    {
                        Physics.IgnoreCollision(collider, builtCollider);
                    }
                }

                if(builtObject.name.Contains("Transmitter") && Builder.placementTarget.GetComponentInParent<Base>() is null && finalTarget.TryGetComponent(out LargeWorldEntity largeWorldEntity2))
                {
                    largeWorldEntity2.cellLevel = LargeWorldEntity.CellLevel.Global;
                    largeWorldEntity2.initialCellLevel = LargeWorldEntity.CellLevel.Global;

                    LargeWorldStreamer.main.cellManager.RegisterEntity(largeWorldEntity2);
                }
                builtObject.transform.SetParent(finalTarget.transform);
            }

            Main.Config.AttachToTarget = false;
        }


        return builtObject.transform;
    }

    private static GameObject GetFinalTarget(GameObject placementTarget)
    {
        GameObject finalTarget = null;

        if(placementTarget == null)
            return finalTarget;

        var pickupable = placementTarget.GetComponentInParent<Pickupable>();
        if(pickupable != null)
        {
            finalTarget = pickupable.gameObject;
        }
        else
        {
            var creature = placementTarget.GetComponentInParent<Creature>();
            if(creature != null)
            {
                finalTarget = creature.gameObject;
            }
            else
            {
                var subRoot = placementTarget.GetComponentInParent<SubRoot>();
                if(subRoot != null)
                {
                    finalTarget = subRoot.modulesRoot.gameObject;
                }
                else
                {
                    var vehicle = placementTarget.GetComponentInParent<Vehicle>();
                    if(vehicle != null)
                    {
                        finalTarget = vehicle.modulesRoot.gameObject;
                    }
                    else
                    {

                        Component lifepod =
#if SUBNAUTICA
                        placementTarget.GetComponentInParent<EscapePod>();
#elif BELOWZERO
                        placementTarget.GetComponentInParent<LifepodDrop>();
#endif
                        if(lifepod != null)
                        {
                            finalTarget = lifepod.gameObject;
                        }
#if BELOWZERO
                        else
                        {
                            var seaTruck = placementTarget.GetComponentInParent<SeaTruckSegment>();
                            if(seaTruck != null)
                                finalTarget = seaTruck.gameObject;
                        }
#endif
                    }
                }
            }
        }

        return finalTarget;
    }
}
