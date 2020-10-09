using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMOD;
using HarmonyLib;
using UnityEngine;
using UWE;

namespace DropUpgradesOnDestroy.Patches
{
    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnKill))]
    public class SubRoot_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(SubRoot __instance)
        {
            List<InventoryItem> equipment = __instance.upgradeConsole?.modules.equipment?.Values?.Where((e) => e != null).ToList() ?? new List<InventoryItem>();

            Assembly MCU = AppDomain.CurrentDomain.GetAssemblies()
                                                        .Where((x) => x.FullName.StartsWith("MoreCyclopsUpgrades"))?
                                                        .FirstOrFallback(null);

            if(MCU != null)
            {
                Type IMCUCrossMod = AccessTools.TypeByName("MoreCyclopsUpgrades.API.IMCUCrossMod");
                Type MCUServices = AccessTools.TypeByName("MoreCyclopsUpgrades.API.MCUServices");
                Type UpgradeSlot = AccessTools.TypeByName("MoreCyclopsUpgrades.API.Buildables.UpgradeSlot");
                
                if (IMCUCrossMod != null && UpgradeSlot != null)
                {
                    MethodInfo IMCUCrossMod_GetAllUpgradeSlots = AccessTools.Method(IMCUCrossMod, "GetAllUpgradeSlots");
                    MethodInfo UpgradeSlot_GetItemInSlot = AccessTools.Method(UpgradeSlot, "GetItemInSlot");
                    PropertyInfo CrossMod = AccessTools.Property(MCUServices, "CrossMod");
                    if (IMCUCrossMod_GetAllUpgradeSlots != null && UpgradeSlot_GetItemInSlot != null && CrossMod != null)
                    {
                        var listType = typeof(List<>).MakeGenericType(UpgradeSlot);
                        IList upgradeSlots = (IList)Activator.CreateInstance(listType, IMCUCrossMod_GetAllUpgradeSlots.Invoke(CrossMod.GetValue(null), new object[] { __instance }));

                        foreach(var slot in upgradeSlots)
                        {
                            InventoryItem item = (InventoryItem)UpgradeSlot_GetItemInSlot.Invoke(slot, null);

                            if(item != null)
                            {
                                equipment.Add(item);
                            }
                        }
                    }
                }
            }

            Vector3 position = __instance.gameObject.transform.position;
            Main.SpawnModuleNearby(equipment, position);
        }
    }
}