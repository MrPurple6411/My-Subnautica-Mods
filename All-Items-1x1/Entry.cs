using Harmony;
using System.Reflection;

namespace All_Items_1x1
{
    public class Entry
    {
        public static void Patch()
        {
            HarmonyInstance.Create("MrPurple6411.All_Items_1x1").PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(CraftData))]
    [HarmonyPatch(nameof(CraftData.GetItemSize))]
    internal class Resizer
    {
        [HarmonyPostfix]
        public static void postfix(TechType techType, ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
}
