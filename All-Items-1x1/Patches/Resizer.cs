using HarmonyLib;
#if SUBNAUTICA
using Data = CraftData;
#elif BELOWZERO
using Data = TechData;
#endif

namespace All_Items_1x1.Patches
{
    [HarmonyPatch(typeof(Data), nameof(Data.GetItemSize))]
    public class Resizer
    {
        [HarmonyPostfix]
        public static void Postfix(ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
}
