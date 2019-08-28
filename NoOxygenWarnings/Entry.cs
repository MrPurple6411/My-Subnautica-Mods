using System.Reflection;
using Harmony;

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
