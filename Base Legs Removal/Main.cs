namespace Base_Legs_Removal
{
    using Base_Legs_Removal.Configuration;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}