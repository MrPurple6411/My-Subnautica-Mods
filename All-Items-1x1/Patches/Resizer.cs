namespace AllItems1x1.Patches;

using HarmonyLib;

[HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize))]
public class Resizer
{
    [HarmonyPostfix]
    public static void Postfix(ref Vector2int __result)
    {
        __result = new Vector2int(1, 1);
    }
}
