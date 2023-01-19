namespace BuilderModule
{
    using Module;
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Reflection;
#if BZ
    using SMLHelper.Handlers;
#endif

    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "BuilderModule",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static ManualLogSource logSource;
        internal static readonly List<TechType> builderModules = new();
        #endregion

        private void Awake()
        {
            logSource = Logger;
            var builderModule = new BuilderModulePrefab();
            builderModules.Add(builderModule.PrefabInfo.TechType);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}