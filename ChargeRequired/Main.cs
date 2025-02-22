namespace ChargeRequired;

using Patches;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main: BaseUnityPlugin
{
    internal static Harmony harmony;
    internal static PropertyInfo containers;

    private void Awake()
    {
        harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

        var EasyCraft = AppDomain.CurrentDomain.GetAssemblies()
                                                    .Where((x) => x.FullName.StartsWith("EasyCraft"))
                                                    .FirstOrFallback(null);

        if (EasyCraft == null) return;
        var ClosestItemContainers = AccessTools.TypeByName("ClosestItemContainers");
        if (ClosestItemContainers == null) return;
        containers = AccessTools.Property(ClosestItemContainers, "containers");
        if (containers == null) return;
        var ClosestItemContainers_GetPickupCount = AccessTools.Method(ClosestItemContainers, "GetPickupCount");
        var ClosestItemContainers_DestroyItem = AccessTools.Method(ClosestItemContainers, "DestroyItem");
        if (ClosestItemContainers_GetPickupCount == null || ClosestItemContainers_DestroyItem == null) return;
        harmony.Patch(ClosestItemContainers_GetPickupCount, prefix: new HarmonyMethod(typeof(ClosestItemContainers_Patches), nameof(ClosestItemContainers_Patches.ClosestItemContainers_GetPickupCount_Prefix)));
        harmony.Patch(ClosestItemContainers_DestroyItem, prefix: new HarmonyMethod(typeof(ClosestItemContainers_Patches), nameof(ClosestItemContainers_Patches.ClosestItemContainers_DestroyItem_Prefix)));
        Logger.Log(LogLevel.Info, "Successfully Patched EasyCraft Methods.");
    }

    public static bool BatteryCheck(Pickupable pickupable)
    {
        var energyMixin = pickupable.gameObject.GetComponentInChildren<EnergyMixin>();
        if(energyMixin != null)
        {
            var gameObject = energyMixin.GetBatteryGameObject();
            if(gameObject != null && energyMixin.defaultBattery == CraftData.GetTechType(gameObject))
            {
                var battery = gameObject.GetComponent<IBattery>();
                return Math.Abs(battery.capacity - battery.charge) < 0.001f;
            }
            else if(gameObject == null && BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Any(x => x.Metadata.Name == "NoBattery"))
            {
                return true;
            }
            return false;
        }

        var b2 = pickupable.GetComponent<IBattery>();
        return b2 == null || Math.Abs(b2.capacity - b2.charge) < 0.001f;
    }
}