using Harmony;
using System.Reflection;

namespace NoOxygenWarnings
{
    public class Entry
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.NoOxygenWarnings");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
