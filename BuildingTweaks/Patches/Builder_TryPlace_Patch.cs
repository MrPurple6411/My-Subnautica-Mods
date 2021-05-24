﻿namespace BuildingTweaks.Patches
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
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            bool found = false;
            bool found2 = false;
            List<OpCode> opCodes = new List<OpCode>() { OpCodes.Ldloc_2, OpCodes.Ldloc_3 };

            for(int i = 0; i < instructions.Count() - 2; i++)
            {
                CodeInstruction currentInstruction = codeInstructions[i];
                CodeInstruction secondInstruction = codeInstructions[i + 1];
                CodeInstruction thirdInstruction = codeInstructions[i + 2];

                if(!found && currentInstruction.opcode == OpCodes.Ldloc_0 && secondInstruction.opcode == OpCodes.Ldc_I4_0 && thirdInstruction.opcode == OpCodes.Ldc_I4_1)
                {

                    codeInstructions.Insert(i + 1, new CodeInstruction(OpCodes.Call, typeof(Builder_TryPlace_Patch).GetMethod(nameof(Builder_TryPlace_Patch.SetBaseParent))));
                    found = true;
                    continue;
                }

                if(opCodes.Contains(currentInstruction.opcode) && secondInstruction.opcode == OpCodes.Callvirt && thirdInstruction.opcode == OpCodes.Dup)
                {
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, typeof(Builder_TryPlace_Patch).GetMethod(nameof(Builder_TryPlace_Patch.SetParent)));
                    found2 = true;
                    break;
                }
                continue;
            }

            if(found is false || found2 is false)
                Logger.Log(Logger.Level.Error, $"Cannot find patch locations {found}:{found2} in Builder.TryPlace");
            else
                Logger.Log(Logger.Level.Info, "Transpiler for Builder.TryPlace completed");

            return codeInstructions.AsEnumerable();
        }

        public static ConstructableBase SetBaseParent(ConstructableBase constructableBase)
        {
            BaseGhost baseGhost = constructableBase.gameObject.GetComponentInChildren<BaseGhost>();

            if(Main.Config.AttachToTarget && baseGhost != null && baseGhost.TargetBase == null && Builder.placementTarget != null)
            {
                GameObject placementTarget = UWE.Utils.GetEntityRoot(Builder.placementTarget) ?? Builder.placementTarget;
                if(placementTarget.TryGetComponent(out LargeWorldEntity largeWorldEntity))
                {
                    largeWorldEntity.cellLevel = LargeWorldEntity.CellLevel.Global;
                    largeWorldEntity.initialCellLevel = LargeWorldEntity.CellLevel.Global;

                    LargeWorldStreamer.main.cellManager.RegisterEntity(largeWorldEntity);
                }

                if(constructableBase.gameObject.TryGetComponent(out Collider builtCollider))
                {
                    foreach(Collider collider in placementTarget.GetComponentsInChildren<Collider>() ?? new Collider[0])
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
            LargeWorldEntity largeWorldEntity = builtObject.GetComponent<LargeWorldEntity>();
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
                Transform aimTransform = Builder.GetAimTransform();
                if(Physics.Raycast(aimTransform.position, aimTransform.forward, out RaycastHit hit, Builder.placeMaxDistance, Builder.placeLayerMask.value, QueryTriggerInteraction.Ignore))
                {
                    Collider hitCollider = hit.collider;
                    Builder.placementTarget = hitCollider.gameObject;
                }
            }

            if(Main.Config.AttachToTarget || (Builder.placementTarget is not null && builtObject.GetComponent<ConstructableBase>() is null))
            {
                GameObject placementTarget = Builder.placementTarget is not null ? UWE.Utils.GetEntityRoot(Builder.placementTarget) ?? Builder.placementTarget : null;
                GameObject finalTarget = null;

                if(placementTarget != null)
                {
                    Pickupable pickupable = placementTarget.GetComponentInParent<Pickupable>();
                    if(pickupable != null)
                    {
                        finalTarget = pickupable.gameObject;
                    }
                    else
                    {
                        Creature creature = placementTarget.GetComponentInParent<Creature>();
                        if(creature != null)
                        {
                            finalTarget = creature.gameObject;
                        }
                        else
                        {
                            SubRoot subRoot = placementTarget.GetComponentInParent<SubRoot>();
                            if(subRoot != null)
                            {
                                finalTarget = subRoot.modulesRoot.gameObject;
                            }
                            else
                            {
                                Vehicle vehicle = placementTarget.GetComponentInParent<Vehicle>();
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
                                        SeaTruckSegment seaTruck = placementTarget.GetComponentInParent<SeaTruckSegment>();
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
                    foreach(Collider builtCollider in builtObject.GetComponentsInChildren<Collider>()?? new Collider[0])
                    {
                        foreach(Collider collider in finalTarget.GetComponentsInChildren<Collider>() ?? new Collider[0])
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
            }
            

            return builtObject.transform;
        }
    }

}
