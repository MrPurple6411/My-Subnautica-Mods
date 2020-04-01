using Harmony;
using QModManager.API.ModLoading;
using System.Reflection;

namespace QModTemplate
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"YourName_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}