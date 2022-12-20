namespace BuilderModule
{
    using Module;
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Reflection;
#if BZ
    using SMLHelper.V2.Handlers;
#endif

    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static readonly List<TechType> builderModules = new();
#if BZ
        internal static FieldInfo AttachToTargetField;
        internal static object btConfig;
#endif

        #region[Declarations]

        public const string
            MODNAME = "BuilderModule",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static ManualLogSource logSource;

        #endregion

        private void Awake()
        {
            logSource = Logger;
            var builderModule = new BuilderModulePrefab("BuilderModule", "Builder Module", "Allows you to build bases while in your vehicle.", new[] { "Upgrades", "ExosuitUpgrades" }, EquipmentType.VehicleModule);
            builderModule.Patch();
            builderModules.Add(builderModule.TechType);

#if BZ
            if(BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Any(x => x.Metadata.Name == "BuildingTweaks"))
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