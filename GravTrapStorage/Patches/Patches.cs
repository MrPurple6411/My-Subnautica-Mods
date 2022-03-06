using SMLHelper.V2.Utility;
using UWE;

namespace GravTrapStorage.Patches
{
    using UnityEngine;
    using System.Collections.Generic;
    using HarmonyLib;
    
    [HarmonyPatch]
    internal class Patches
    {
        public static readonly Dictionary<Gravsphere, StorageContainer> StorageContainers = new();
        public static Dictionary<Gravsphere, bool> ResetTriggers = new Dictionary<Gravsphere, bool>();

        [HarmonyPatch(typeof(Gravsphere), nameof(Gravsphere.Start))]
        public static void Prefix(Gravsphere __instance)
        {
            StorageContainer storageContainer = __instance.transform.GetChild(0)?.GetComponent<StorageContainer>();
            if (storageContainer != null)
            {
                storageContainer.container.Resize(Main.ConfigFile.Width, Main.ConfigFile.Height);
                StorageContainers[__instance] = storageContainer;
                ResetTriggers[__instance] = false;
            }
        }
    
        [HarmonyPatch(typeof(Gravsphere), nameof(Gravsphere.Update))]
        public static void Postfix(Gravsphere __instance)
        {
            if (Inventory.main.GetHeldTool() != __instance || !StorageContainers.TryGetValue(__instance, out StorageContainer storageContainer) ||
                storageContainer == null) return;

            if(storageContainer.container.allowedTech != new HashSet<TechType>(Main.ConfigFile.AllowedTechTypes))
                storageContainer.container.SetAllowedTechTypes(Main.ConfigFile.AllowedTechTypes.ToArray());
            
            if (__instance.attractableList.Count == 0 && __instance.trigger.enabled)
            {
                    __instance.trigger.enabled = false;
                    ResetTriggers[__instance] = true;
            }

            if (ResetTriggers[__instance])
            {
                __instance.trigger.enabled = true;
                ResetTriggers[__instance] = false;
            }
            

            if (Targeting.GetTarget(Player.main.gameObject, Main.ConfigFile.TransferDistance, out var activeTarget, out var activeHitDistance) &&
                (UWE.Utils.GetEntityRoot(activeTarget) ?? activeTarget).TryGetComponent(
                    out StorageContainer targetContainer))
            {

                string targetName = CraftData.GetTechType(targetContainer.gameObject).AsString();
                
                if (!storageContainer.GetOpen())
                {
                    string transferString = targetContainer.container.IsFull()
                        ? $"Cannot transfer as {targetName} is full."
                        : $"Transfer Gravtrap contents to {targetName}";
#if SN1
                    HandReticle.main.interactText2 = 
#else
                    HandReticle.main.textUseSubscript = 
#endif
                        $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", transferString, uGUI.FormatButton(GameInput.Button.AltTool))}";
                }
                if (GameInput.GetButtonDown(GameInput.Button.AltTool))
                {
                    if (targetContainer != null)
                    {
                        foreach (ItemsContainer.ItemGroup itemGroup in new List<ItemsContainer.ItemGroup>(storageContainer.container._items.Values))
                        {
                            foreach (InventoryItem inventoryItem in new List<InventoryItem>(itemGroup.items))
                            {
                                if (((IItemsContainer)targetContainer.container).AddItem(inventoryItem))
                                {
                                    ErrorMessage.AddMessage($"Moved {inventoryItem.item.GetTechName()} to {targetName}");
                                }
                            }
                        }
                    }
                }
            }
            else if(!Player.main.IsInside())
            {
                if (!storageContainer.GetOpen())
                {
                    string gravtrapactivate =
                        storageContainer.container.IsFull() ? "Cannot Activate, Storage is Full" : !__instance.trigger.enabled? "Activate Gravtrap":"Deactivate Gravtrap";
#if SN1
                    HandReticle.main.interactText1 = 
#else
                    HandReticle.main.textUse = 
#endif
                        $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Deploy Gravtrap", uGUI.FormatButton(GameInput.Button.RightHand))}";
                        
#if SN1
                    HandReticle.main.interactText2 = 
#else
                    HandReticle.main.textUseSubscript = 
#endif
                        $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", gravtrapactivate, uGUI.FormatButton(GameInput.Button.LeftHand))}\n{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Open Storage", uGUI.FormatButton(GameInput.Button.AltTool))}";
                }

                if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
                {
                    if (!__instance.trigger.enabled)
                    {
                        __instance.trigger.enabled = true;
                        __instance.gameObject.GetComponent<Pickupable>().attached = false;
                    }
                    else
                    {
                        __instance.DeactivatePads();
                        __instance.trigger.enabled = false;
                        __instance.gameObject.GetComponent<Pickupable>().attached = true;
                    }
                }
                
                if (GameInput.GetButtonDown(GameInput.Button.AltTool) && !IngameMenu.main.selected)
                {
                    storageContainer.Open(__instance.transform);
                }
            }
            else
            {
            
                if (!storageContainer.GetOpen())
                {
#if SN1
                    HandReticle.main.interactText2 = 
#else
                    HandReticle.main.textUseSubscript = 
#endif
                        $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Open Storage", uGUI.FormatButton(GameInput.Button.AltTool))}";
                }
            
                if (GameInput.GetButtonDown(GameInput.Button.AltTool) && !IngameMenu.main.selected)
                {
                    storageContainer.Open(__instance.transform);
                }
            }

            
            if (storageContainer.container.IsFull())
            {
                __instance.DeactivatePads();
                __instance.trigger.enabled = false;
                __instance.gameObject.GetComponent<Pickupable>().attached = true;
            }
            
        }
        
        [HarmonyPatch(typeof(Gravsphere), nameof(Gravsphere.IsValidTarget))]
        public static void Postfix(Gravsphere __instance, GameObject obj, ref bool __result)
        {
            if(!__result || !StorageContainers.TryGetValue(__instance, out StorageContainer container))
                return;

            if (Vector3.Distance(__instance.transform.position, obj.transform.position) > Main.ConfigFile.Distance) return;
            
            if (obj.TryGetComponent(out Pickupable pickupable) && container.container.HasRoomFor(pickupable))
            {
                if (container.container.AddItem(pickupable) == null) return;
                pickupable.PlayPickupSound();
                pickupable.Deactivate();
                pickupable.transform.SetParent(container.transform);
                __result = false;
            }
            else if(obj.TryGetComponent(out BreakableResource resource))
            {
                resource.BreakIntoResources();
                __result = false;
            }
            else if(pickupable == null && Inventory.main.GetHeldTool() == __instance)
            {
                __result = false;
            }
        }
    }
}
