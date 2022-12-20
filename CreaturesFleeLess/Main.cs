namespace CreaturesFleeLess
{
    using HarmonyLib;
    using System.Reflection;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "CreaturesFleeLess",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        internal readonly Harmony harmony;
        internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
        public readonly string modFolder;

        #endregion

        private void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}