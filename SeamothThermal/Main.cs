using BepInEx;

namespace SeamothThermal
{
    using HarmonyLib;
    using System;
    using System.Reflection;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        internal static Modules.SeamothThermalModule thermalModule = new();

        public void Start()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

            thermalModule.Patch();

            Console.WriteLine("[SeamothThermal] Succesfully patched!");
        }
    }
}