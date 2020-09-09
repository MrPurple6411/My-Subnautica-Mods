using HarmonyLib;
#if SN1
using Data = CraftData;
#elif BZ
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
