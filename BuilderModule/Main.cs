namespace BuilderModule
{
    using BuilderModule.Module;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        internal static BuilderModulePrefab buildermodule = new BuilderModulePrefab();

        [QModPatch]
        public static void Load()
        {
            buildermodule.Patch();

            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}