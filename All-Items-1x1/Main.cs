using Harmony;
using QModManager.API.ModLoading;
using System.Reflection;

namespace All_Items_1x1
{
    [HarmonyPatch(typeof(CraftData))]
    [HarmonyPatch(nameof(CraftData.GetItemSize))]
    public class Resizer
    {
        [HarmonyPostfix]
        public static void Postfix(ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
}
