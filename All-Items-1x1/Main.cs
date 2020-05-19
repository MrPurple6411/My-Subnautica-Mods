using System.Reflection;
using Harmony;
using QModManager.API.ModLoading;

namespace All_Items_1x1
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
#if SUBNAUTICA

    [HarmonyPatch(typeof(CraftData), nameof(CraftData.GetItemSize))]
    public class Resizer
    {
        [HarmonyPostfix]
        public static void Postfix(ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
#elif BELOWZERO
    [HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize))]
    public class Resizer
    {
        [HarmonyPostfix]
        public static void Postfix(ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
#endif
}