using Harmony;
using System.Reflection;

namespace SaveTheSunbeam
{
    public static class Mod
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.SaveTheSunbeam");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}