using System.Reflection;
using Harmony;

namespace MrPurple
{
	public class Entry
	{
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("subnautica.mrpurple.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
