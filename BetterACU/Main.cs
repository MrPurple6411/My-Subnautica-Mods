using BepInEx;

namespace BetterACU
{
    using Configuration;
    using HarmonyLib;
    using SMCLib.Handlers;
    using System.Reflection;

    public  class Main:BaseUnityPlugin
    {
        internal static Config SmcConfig { get; private set; }

        public  void Awake()
        {
            SmcConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            IngameMenuHandler.RegisterOnSaveEvent(SmcConfig.Save);

            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}