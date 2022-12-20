namespace SeamothThermal
{
    using HarmonyLib;
    using System;
    using System.Reflection;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static Modules.SeamothThermalModule thermalModule = new();

        #region[Declarations]

        public const string
            MODNAME = "SeamothThermal",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        #endregion

        private void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

            thermalModule.Patch();

            Console.WriteLine("[SeamothThermal] Succesfully patched!");
        }
    }
}