namespace PersistentCommands
{
    using HarmonyLib;
    using Configuration;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}