namespace PersistentCommands
{
    using HarmonyLib;
    using Configuration;
    using SMLHelper.Handlers;    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "PersistentCommands",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.1";
        internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        #endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Patches.Patches), GUID);
        }
    }
}