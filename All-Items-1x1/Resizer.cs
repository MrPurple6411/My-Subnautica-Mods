using HarmonyLib;

namespace All_Items_1x1
{
#if SUBNAUTICA
    [HarmonyPatch(typeof(CraftData), nameof(CraftData.GetItemSize))]
#elif BELOWZERO
    [HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize))]
#endif
    public class Resizer
    {
        [HarmonyPostfix]
        public static void Postfix(ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
}
