using System.Reflection;
using Harmony;

namespace ResourceOverload
{
    public class Entry
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.ResourceOverload");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
