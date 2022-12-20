namespace ChargeRequired
{
    using Patches;
    using HarmonyLib;
    using System;
    using System.Linq;
    using System.Reflection;

    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static PropertyInfo containers;

        #region[Declarations]

        public const string
            MODNAME = "ChargeRequired",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        internal readonly Harmony harmony;
        internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
        public readonly string modFolder;

        #endregion

        private void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

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
}