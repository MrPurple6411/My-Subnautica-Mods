namespace ToolInspection
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}