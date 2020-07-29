using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;

namespace BetterACU
{
    [QModCore]
    public static class Main
    {
        internal static Config config = new Config();

        [QModPatch]
        public static void Load()
        {
            config.Load();
            IngameMenuHandler.RegisterOnSaveEvent(() => config.Save());
            OptionsPanelHandler.RegisterModOptions(new Options());

            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}