using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;

namespace NoOxygenWarnings
{
    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}
