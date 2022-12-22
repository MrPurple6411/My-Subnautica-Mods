namespace Time_Eternal
{
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using System.Reflection;
    using Configuration;    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "Time_Eternal",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        #endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}