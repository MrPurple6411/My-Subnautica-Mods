using BepInEx;

namespace SeamothThermal
{
    using HarmonyLib;
    using System;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        internal static Modules.SeamothThermalModule thermalModule = new();

        public void  Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

            thermalModule.Patch();

            Console.WriteLine("[SeamothThermal] Succesfully patched!");
        }
    }
}