using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using QModManager.API;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BuilderModule
{
    public static class Main
    {
        public static string inputFixFolder = Path.Combine(SNUtils.applicationRootDir, "QMods/BuilderModuleInputFix");
        public static void Load()
        {
            try
            {
                var buildermodule = new BuilderModulePrefab();
                buildermodule.Patch();
                HarmonyInstance.Create("MrPurple6411.BuilderModule").PatchAll(Assembly.GetExecutingAssembly());

                if (Directory.Exists(inputFixFolder))
                {
                    QModServices.Main.AddCriticalMessage($"This mod no longer requires the BuilderModuleInputFix and that mod needs to be removed from your Qmods folder.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Vehicle_OnUpgradeModuleChange_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Vehicle __instance, int slotID, TechType techType, bool added)
        {
            if (techType == BuilderModulePrefab.TechTypeID && added)
            {
                if (__instance.GetType() == typeof(SeaMoth))
                {
                    BuilderModule seamoth_control = __instance.gameObject.GetOrAddComponent<BuilderModule>();
                    seamoth_control.ModuleSlotID = slotID;
                    return;
                }
                else if (__instance.GetType() == typeof(Exosuit))
                {
                    BuilderModule exosuit_control = __instance.gameObject.GetOrAddComponent<BuilderModule>();
                    exosuit_control.ModuleSlotID = slotID;
                    return;
                }
                else
                {
                    Debug.Log("[BuilderModule] Error! Unidentified Vehicle Type!");
                }
            }
        }
    }


    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch("Update")]
    internal class Builder_Update_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int codepoint = -1;

            object inputHandler = AccessTools.Field(typeof(Builder), "inputHandler");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction currentCode = codes[i];
                if (codepoint == -1)
                {
                    if (currentCode.opcode == OpCodes.Ldsfld && currentCode.operand == inputHandler)
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
            BuildModeInputHandler inputHandler = (BuildModeInputHandler)AccessTools.Field(typeof(Builder), "inputHandler").GetValue(null);
            if (Player.main.GetVehicle() == null)
            {
                inputHandler.canHandleInput = true;
                InputHandlerStack.main.Push(inputHandler);
            }
            else
            {
                inputHandler.canHandleInput = false;
            }
        }
    }

    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch("Construct")]
    internal class Construct_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Constructable __instance)
        {
            if (Player.main.GetVehicle() != null && GameModeUtils.RequiresIngredients())
            {
                Type ConstructableType = typeof(Constructable);
                List<TechType> resourceMapField = (List<TechType>)AccessTools.Field(ConstructableType, "resourceMap").GetValue(__instance);
                MethodInfo getResourceIDMethod = AccessTools.Method(ConstructableType, "GetResourceID");
                MethodInfo GetConstructIntervalMethod = AccessTools.Method(ConstructableType, "GetConstructInterval");
                MethodInfo updateMaterialMethod = AccessTools.Method(ConstructableType, "UpdateMaterial");


                Vehicle thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return false;
                }
                int count = resourceMapField.Count;
                int resourceID = (int)getResourceIDMethod.Invoke(__instance, null);
                float backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount += Time.deltaTime / (count * (float)GetConstructIntervalMethod.Invoke(__instance, null));
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = (int)getResourceIDMethod.Invoke(__instance, null);
                if (resourceID2 != resourceID)
                {
                    TechType destroyTechType = resourceMapField[resourceID2 - 1];
                    if (thisVehicle.GetType().Equals(typeof(Exosuit)))
                    {
                        StorageContainer storageContainer = ((Exosuit)thisVehicle).storageContainer;

                        if (storageContainer.container.Contains(destroyTechType))
                        {
                            _ = storageContainer.container.DestroyItem(destroyTechType);
                        }
                        else
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                    else
                    {
                        var seamoth = (SeaMoth)thisVehicle;
                        bool storageCheck = false;
                        for (int i = 0; i < 12; i++)
                        {
                            try
                            {
                                ItemsContainer storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                if (storage != null && storage.Contains(destroyTechType))
                                {
                                    if (storage.DestroyItem(destroyTechType))
                                    {
                                        storageCheck = true;
                                        break;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        if (!storageCheck)
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                }
                _ = updateMaterialMethod.Invoke(__instance, null);
                if (__instance.constructedAmount >= 1f)
                {
                    _ = __instance.SetState(true, true);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch("Deconstruct")]
    internal class Deconstruct_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Constructable __instance)
        {
            if (Player.main.GetVehicle() != null && GameModeUtils.RequiresIngredients())
            {
                Type ConstructableType = typeof(Constructable);
                List<TechType> resourceMapField = (List<TechType>)AccessTools.Field(ConstructableType, "resourceMap").GetValue(__instance);
                MethodInfo getResourceIDMethod = AccessTools.Method(ConstructableType, "GetResourceID");
                MethodInfo GetConstructIntervalMethod = AccessTools.Method(ConstructableType, "GetConstructInterval");
                MethodInfo updateMaterialMethod = AccessTools.Method(ConstructableType, "UpdateMaterial");

                Vehicle thisVehicle = Player.main.GetVehicle();
                if (__instance._constructed)
                {
                    return true;
                }
                int count = resourceMapField.Count;

                int resourceID = (int)getResourceIDMethod.Invoke(__instance, null);
                float backupConstructedAmount = __instance.constructedAmount;
                __instance.constructedAmount -= Time.deltaTime / (count * (float)GetConstructIntervalMethod.Invoke(__instance, null));
                __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
                int resourceID2 = (int)getResourceIDMethod.Invoke(__instance, null);
                if (resourceID2 != resourceID)
                {
                    TechType techType = resourceMapField[resourceID2];
                    GameObject gameObject = CraftData.InstantiateFromPrefab(techType, false);
                    Pickupable component = gameObject.GetComponent<Pickupable>();

                    if (thisVehicle.GetType().Equals(typeof(Exosuit)))
                    {
                        StorageContainer storageContainer = ((Exosuit)thisVehicle).storageContainer;

                        if (storageContainer.container.HasRoomFor(component))
                        {
                            string name = Language.main.Get(component.GetTechName());
                            ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));

                            uGUI_IconNotifier.main.Play(component.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);
#if SUBNAUTICA
                            component = component.Initialize();
#elif BELOWZERO
                            component.Initialize();
#endif
                            var item = new InventoryItem(component);
                            storageContainer.container.UnsafeAdd(item);
                            component.PlayPickupSound();
                        }
                        else
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                    else
                    {
                        var seamoth = (SeaMoth)thisVehicle;
                        bool storageCheck = false;
                        for (int i = 0; i < 12; i++)
                        {
                            try
                            {
                                ItemsContainer storage = seamoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                if (storage != null && storage.HasRoomFor(component))
                                {
                                    string name = Language.main.Get(component.GetTechName());
                                    ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));

                                    uGUI_IconNotifier.main.Play(component.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);

#if SUBNAUTICA
                                    component = component.Initialize();
#elif BELOWZERO
                                    component.Initialize();
#endif
                                    var item = new InventoryItem(component);
                                    storage.UnsafeAdd(item);
                                    component.PlayPickupSound();
                                    storageCheck = true;
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        if (!storageCheck)
                        {
                            __instance.constructedAmount = backupConstructedAmount;
                            return true;
                        }
                    }
                }
                updateMaterialMethod.Invoke(__instance, null);
                return __instance.constructedAmount <= 0f;
            }
            else
            {
                return true;
            }
        }
    }
}