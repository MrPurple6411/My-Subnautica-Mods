namespace BuilderModule
{
    using Module;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System.Collections.Generic;
    using System.Reflection;
#if BZ
    using QModManager.API;
    using SMLHelper.V2.Handlers;
#endif

    [QModCore]
    public static class Main
    {
        internal static readonly List<TechType> builderModules = new();
#if BZ
        internal static FieldInfo AttachToTargetField;
        internal static object btConfig;
#endif
        
        [QModPatch]
        public static void Load()
        {
            var builderModule = new BuilderModulePrefab("BuilderModule", "Builder Module", "Allows you to build bases while in your vehicle.", new[] { "Upgrades", "ExosuitUpgrades" }, EquipmentType.VehicleModule);
            builderModule.Patch();
            builderModules.Add(builderModule.TechType);

#if BZ
            if(QModServices.Main.ModPresent("BuildingTweaks"))
            {
                var buildingTweaks = QModServices.Main.GetMod("BuildingTweaks").LoadedAssembly;

                var btMainType = buildingTweaks.GetType("BuildingTweaks.Main", true, true);
                var btConfigType = buildingTweaks.GetType("BuildingTweaks.Configuration.Config", true, true);

                var btMainConfigProperty = btMainType.GetProperty("Config", btConfigType);
                AttachToTargetField = btConfigType.GetField("AttachToTarget", BindingFlags.Public | BindingFlags.Instance);

                if (btMainConfigProperty is not null) btConfig = btMainConfigProperty.GetValue(null);
            }

            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "HoverbikeUpgrades", "Snowfox Upgrades", SpriteManager.Get(TechType.Hoverbike), new string[] { "Upgrades" });
            var builderModuleSeaTruck = new BuilderModulePrefab("BuilderModuleSeaTruck", "SeaTruck Builder Module", "Allows you to build bases while in your vehicle.", new[] { "Upgrades", "SeatruckUpgrades" }, EquipmentType.SeaTruckModule);
            //var builderModuleHoverBike = new BuilderModulePrefab("BuilderModuleHoverBike", "Snowfox Builder Module", "Allows you to build bases while in your vehicle.", new string[] { "Upgrades", "HoverbikeUpgrades" }, EquipmentType.HoverbikeModule);

            builderModuleSeaTruck.Patch();
            //builderModuleHoverBike.Patch();

            builderModules.Add(builderModuleSeaTruck.TechType);
            //builderModules.Add(builderModuleHoverBike.TechType);
#endif
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "MrPurple6411_BuilderModule");
        }
    }
}