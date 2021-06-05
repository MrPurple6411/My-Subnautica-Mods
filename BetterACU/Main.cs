namespace BetterACU
{
    using Configuration;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        internal static Config Config { get; private set; }

        [QModPatch]
        public static void Load()
        {
            Config = OptionsPanelHandler.RegisterModOptions<Config>();
            IngameMenuHandler.RegisterOnSaveEvent(Config.Save);

            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}