namespace All_Items_1x1.Patches
{
    using HarmonyLib;
#if SN1
    using TechData = CraftData;
#endif

    [HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize))]
    public class Resizer
    {
        [HarmonyPostfix]
        public static void Postfix(ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
}
