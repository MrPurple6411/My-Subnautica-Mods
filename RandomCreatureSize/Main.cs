namespace RandomCreatureSize
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using Configuration;
    using SMLHelper.V2.Handlers;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static CreatureConfig CreatureConfig;

        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"Coticvo_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}