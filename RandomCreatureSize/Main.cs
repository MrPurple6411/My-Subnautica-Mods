using BepInEx;

namespace RandomCreatureSize
{
    using HarmonyLib;
    using Configuration;
    using SMCLib.Handlers;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static CreatureConfig CreatureConfig;

        public void  Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"Coticvo_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}