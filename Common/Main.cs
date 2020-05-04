using System.Reflection;
using Harmony;
using QModManager.API.ModLoading;

namespace Common
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}