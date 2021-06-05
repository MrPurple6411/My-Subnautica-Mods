namespace DropUpgradesOnDestroy.Patches
{
    using HarmonyLib;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnKill))]
    public class SubRoot_OnKill
    {
        [HarmonyPrefix]
        public static void Prefix(SubRoot __instance)
        {
            if(__instance.upgradeConsole == null || __instance.upgradeConsole.modules.equipment?.Values is null) return;
            
            var equipment = __instance.upgradeConsole.modules.equipment.Values.Where((e) => e != null).ToList();

            var MCU = AppDomain.CurrentDomain.GetAssemblies()
                                                        .Where((x) => x.FullName.StartsWith("MoreCyclopsUpgrades"))
                                                        .FirstOrFallback(null);

            if(MCU != null)
            {
                var IMCUCrossMod = AccessTools.TypeByName("MoreCyclopsUpgrades.API.IMCUCrossMod");
                var MCUServices = AccessTools.TypeByName("MoreCyclopsUpgrades.API.MCUServices");
                var UpgradeSlot = AccessTools.TypeByName("MoreCyclopsUpgrades.API.Buildables.UpgradeSlot");

                if(IMCUCrossMod != null && UpgradeSlot != null)
                {
                    var IMCUCrossMod_GetAllUpgradeSlots = AccessTools.Method(IMCUCrossMod, "GetAllUpgradeSlots");
                    var UpgradeSlot_GetItemInSlot = AccessTools.Method(UpgradeSlot, "GetItemInSlot");
                    var CrossMod = AccessTools.Property(MCUServices, "CrossMod");
                    if(IMCUCrossMod_GetAllUpgradeSlots != null && UpgradeSlot_GetItemInSlot != null && CrossMod != null)
                    {
                        var listType = typeof(List<>).MakeGenericType(UpgradeSlot);
                        var upgradeSlots = (IList)Activator.CreateInstance(listType, IMCUCrossMod_GetAllUpgradeSlots.Invoke(CrossMod.GetValue(null), new object[] { __instance }));

                        equipment.AddRange(from object slot in upgradeSlots select (InventoryItem) UpgradeSlot_GetItemInSlot.Invoke(slot, null) into item where item != null select item);
                    }
                }
            }

            var position = __instance.gameObject.transform.position;
            Main.SpawnModuleNearby(equipment, position);
        }
    }
}