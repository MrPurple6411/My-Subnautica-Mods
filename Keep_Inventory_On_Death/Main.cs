using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;

namespace Keep_Inventory_On_Death
{
    [QModCore]
    public static class Main
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}