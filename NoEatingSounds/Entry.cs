using System.Reflection;
using Harmony;

namespace MrPurple
{
	public class Entry
	{
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.NoEatingSounds");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
