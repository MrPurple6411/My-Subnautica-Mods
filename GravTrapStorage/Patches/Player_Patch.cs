using System.Linq;
using GravTrapStorage.MonoBehaviours;
using HarmonyLib;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace GravTrapStorage.Patches;

[HarmonyPatch]
public class Player_Patch
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    [HarmonyPostfix]
    public static void Postfix(Player __instance)
    {
        if (__instance._currentSub == null) return;

        
        
        foreach (var itemGroup in Inventory.main._container._items.Values)
        {
            if(itemGroup.items.Count == 0 || !itemGroup.items[0].item.gameObject.GetComponentInChildren<Gravsphere>(true))
                continue;

            foreach (InventoryItem inventoryItem in itemGroup.items)
            {
                StorageContainer storageContainer =
                    inventoryItem.item.gameObject.GetComponentInChildren<StorageContainer>(true);
                if(storageContainer is null || !storageContainer.container.Any()) continue;
            
                uGUI_GravtrapIcon.main.Show();
                return;
            }
        }
        
    }
    
}