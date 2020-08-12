using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;

namespace JunkDebugging
{
    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

    }

}