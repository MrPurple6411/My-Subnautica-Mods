namespace ChargeRequired
{
    using ChargeRequired.Patches;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;

    [QModCore]
    public static class Main
    {
        internal static PropertyInfo containers;

        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony harmony = Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");

            Assembly EasyCraft = AppDomain.CurrentDomain.GetAssemblies()
                                                        .Where((x) => x.FullName.StartsWith("EasyCraft"))?
                                                        .FirstOrFallback(null);

            if(EasyCraft != null)
            {
                Type ClosestItemContainers = AccessTools.TypeByName("ClosestItemContainers");
                if(ClosestItemContainers != null)
                {
                    containers = AccessTools.Property(ClosestItemContainers, "containers");
                    if(containers != null)
                    {
                        MethodInfo ClosestItemContainers_GetPickupCount = AccessTools.Method(ClosestItemContainers, "GetPickupCount");
                        MethodInfo ClosestItemContainers_DestroyItem = AccessTools.Method(ClosestItemContainers, "DestroyItem");
                        if(ClosestItemContainers_GetPickupCount != null && ClosestItemContainers_DestroyItem != null)
                        {
                            harmony.Patch(ClosestItemContainers_GetPickupCount, prefix: new HarmonyMethod(typeof(ClosestItemContainers_Patches), nameof(ClosestItemContainers_Patches.ClosestItemContainers_GetPickupCount_Prefix)));
                            harmony.Patch(ClosestItemContainers_DestroyItem, prefix: new HarmonyMethod(typeof(ClosestItemContainers_Patches), nameof(ClosestItemContainers_Patches.ClosestItemContainers_DestroyItem_Prefix)));
                            Logger.Log(Logger.Level.Info, "Succesfully Patched Easycraft Methods.");
                        }
                    }
                }
            }
        }

        public static bool BatteryCheck(Pickupable pickupable)
        {
            EnergyMixin energyMixin = pickupable.gameObject.GetComponentInChildren<EnergyMixin>();
            if(energyMixin != null)
            {
                GameObject gameObject =
#if !BZ
                    energyMixin.GetBattery();
#else
                    energyMixin.GetBatteryGameObject();
#endif
                if(gameObject != null && energyMixin.defaultBattery == CraftData.GetTechType(gameObject))
                {
                    IBattery battery = gameObject.GetComponent<IBattery>();
                    return battery.capacity == battery.charge;
                }
                else if(gameObject == null && QModManager.API.QModServices.Main.ModPresent("NoBattery"))
                {
                    return true;
                }
                return false;
            }

            IBattery b2 = pickupable.GetComponent<IBattery>();
            return b2 == null || b2.capacity == b2.charge;
        }
    }
}