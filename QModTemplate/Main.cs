using System.Reflection;
using Harmony;
using QModManager.API.ModLoading;

namespace QModTemplate
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"YourName_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}