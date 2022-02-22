namespace All_Items_1x1
{
    using HarmonyLib;
    using BepInEx;
#if SN1
    using TechData = CraftData;
#endif

    public class Resizer : BaseUnityPlugin
    {
        public static void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Resizer), PluginInfo.PLUGIN_GUID);
        }

        [HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize))]
        [HarmonyPostfix]
        public static void Postfix(ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
}