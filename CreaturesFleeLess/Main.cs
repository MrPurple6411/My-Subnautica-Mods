namespace CreaturesFleeLess
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System.Reflection;

    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Patch()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}