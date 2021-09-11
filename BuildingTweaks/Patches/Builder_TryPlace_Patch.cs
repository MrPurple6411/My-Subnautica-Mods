namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;

    [HarmonyPatch(typeof(Builder), nameof(Builder.TryPlace))]
    internal class Builder_TryPlace_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var collection = instructions.ToList();
            var codeInstructions = new List<CodeInstruction>(collection);
            var found = false;
            var found2 = false;
            var opCodes = new List<OpCode>() { OpCodes.Ldloc_2, OpCodes.Ldloc_3 };

            for(var i = 0; i < collection.Count() - 2; i++)
            {
                var currentInstruction = codeInstructions[i];
                var secondInstruction = codeInstructions[i + 1];
                var thirdInstruction = codeInstructions[i + 2];

                if(!found && currentInstruction.opcode == OpCodes.Ldloc_0 && secondInstruction.opcode == OpCodes.Ldc_I4_0 && thirdInstruction.opcode == OpCodes.Ldc_I4_1)
                {

                    codeInstructions.Insert(i + 1, new CodeInstruction(OpCodes.Call, typeof(Builder_TryPlace_Patch).GetMethod(nameof(SetBaseParent))));
                    found = true;
                    continue;
                }

                if(opCodes.Contains(currentInstruction.opcode) && secondInstruction.opcode == OpCodes.Callvirt && thirdInstruction.opcode == OpCodes.Dup)
                {
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, typeof(Builder_TryPlace_Patch).GetMethod(nameof(SetParent)));
                    found2 = true;
                    break;
                }
            }

            if(found is false || found2 is false)
                Logger.Log(Logger.Level.Error, $"Cannot find patch locations {found}:{found2} in Builder.TryPlace");
            else
                Logger.Log(Logger.Level.Info, "Transpiler for Builder.TryPlace completed");

            return codeInstructions.AsEnumerable();
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
                GameObject finalTarget = null;

                if(placementTarget != null)
                {
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
#if SN1
                            placementTarget.GetComponentInParent<EscapePod>();
#elif BZ
                            placementTarget.GetComponentInParent<LifepodDrop>();
#endif
                                    if(lifepod != null)
                                    {
                                        finalTarget = lifepod.gameObject;
                                    }
#if BZ
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
                }

                if(finalTarget != null)
                {
                    foreach(var builtCollider in builtObject.GetComponentsInChildren<Collider>()?? new Collider[0])
                    {
                        foreach(var collider in finalTarget.GetComponentsInChildren<Collider>() ?? new Collider[0])
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
    }

}
