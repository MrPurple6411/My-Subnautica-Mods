namespace RandomCreatureSize
{
    using HarmonyLib;
    using Configuration;
    using SMLHelper.V2.Handlers;
    using System.Reflection;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static CreatureConfig CreatureConfig;

        #region[Declarations]

        public const string
            MODNAME = "RandomCreatureSize",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        #endregion

        private void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"Coticvo_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}