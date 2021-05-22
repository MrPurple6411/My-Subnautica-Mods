namespace BuilderModule
{
    using BuilderModule.Module;
    using HarmonyLib;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        internal static List<TechType> builderModules = new List<TechType>();
        internal static FieldInfo AttachToTargetField;
        internal static object btConfig;

        [QModPatch]
        public static void Load()
        {

            if(QModServices.Main.ModPresent("BuildingTweaks"))
            {
                var buildingTweaks = QModServices.Main.GetMod("BuildingTweaks").LoadedAssembly;

                Type btMainType = buildingTweaks.GetType("BuildingTweaks.Main", true, true);
                Type btConfigType = buildingTweaks.GetType("BuildingTweaks.Configuration.Config", true, true);

                PropertyInfo btMainConfigProperty = btMainType.GetProperty("Config", btConfigType);
                AttachToTargetField = btConfigType.GetField("AttachToTarget", BindingFlags.Public | BindingFlags.Instance);

                btConfig = btMainConfigProperty.GetValue(null);
            }

            var builderModule = new BuilderModulePrefab("BuilderModule", "Builder Module", "Allows you to build bases while in your vehicle.", new string[] { "Upgrades", "ExosuitUpgrades" }, EquipmentType.VehicleModule);
            builderModule.Patch();
            builderModules.Add(builderModule.TechType);

#if BZ
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "HoverbikeUpgrades", "Snowfox Upgrades", SpriteManager.Get(TechType.Hoverbike), new string[] { "Upgrades" });
            var builderModuleSeaTruck = new BuilderModulePrefab("BuilderModuleSeaTruck", "SeaTruck Builder Module", "Allows you to build bases while in your vehicle.", new string[] { "Upgrades", "SeatruckUpgrades" }, EquipmentType.SeaTruckModule);
            var builderModuleHoverBike = new BuilderModulePrefab("BuilderModuleHoverBike", "Snowfox Builder Module", "Allows you to build bases while in your vehicle.", new string[] { "Upgrades", "HoverbikeUpgrades" }, EquipmentType.HoverbikeModule);

            builderModuleSeaTruck.Patch();
            builderModuleHoverBike.Patch();

            builderModules.Add(builderModuleSeaTruck.TechType);
            builderModules.Add(builderModuleHoverBike.TechType);
#endif
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "MrPurple6411_BuilderModule");
        }
    }
}