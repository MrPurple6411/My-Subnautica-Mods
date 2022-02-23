namespace All_Items_1x1
{
    using HarmonyLib;
    using BepInEx;
#if SN1
    using TechData = CraftData;
#endif

    
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        public void Start()
        {
            Harmony.CreateAndPatchAll(typeof(Main), PluginInfo.PLUGIN_GUID);
        }

        [HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize))]
        [HarmonyPostfix]
        public static void Postfix(ref Vector2int __result)
        {
            __result = new Vector2int(1, 1);
        }
    }
}