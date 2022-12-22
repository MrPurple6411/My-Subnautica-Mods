namespace BetterACU
{
    using Configuration;
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using System.Reflection;
    using BepInEx;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "BetterACU",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        internal static SMLConfig SMLConfig { get; private set; }
        #endregion


        private void Awake()
        {
            SMLConfig = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
            IngameMenuHandler.RegisterOnSaveEvent(SMLConfig.Save);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}