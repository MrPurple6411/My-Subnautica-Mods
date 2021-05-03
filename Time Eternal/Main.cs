namespace Time_Eternal
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System.Reflection;
    using Time_Eternal.Configuration;

    [QModCore]
    public static class Main
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}