namespace IncreasedChunkDrops
{
    using HarmonyLib;
    using Configuration;
    using SMLHelper.V2.Handlers;
    using System.Reflection;
    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "IncreasedChunkDrops",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        internal static ManualLogSource logSource;
        internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        #endregion

        private void Awake()
        {
            logSource = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}