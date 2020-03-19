using Harmony;
using System.Reflection;

namespace SaveTheSunbeam
{
    public class Entry
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.SaveTheSunbeam");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}