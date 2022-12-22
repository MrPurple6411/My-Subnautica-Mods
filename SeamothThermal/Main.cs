namespace SeamothThermal
{
    using HarmonyLib;
    using System.Reflection;    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "SeamothThermal",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        internal static Modules.SeamothThermalModule thermalModule = new();
        #endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
            thermalModule.Patch();
            Logger.LogInfo("Succesfully patched!");
        }
    }
}