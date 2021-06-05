namespace SeamothThermal
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        internal static Modules.SeamothThermalModule thermalModule = new();

        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

            thermalModule.Patch();

            Console.WriteLine("[SeamothThermal] Succesfully patched!");
        }
    }
}