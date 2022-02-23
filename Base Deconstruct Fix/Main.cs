namespace Base_Deconstruct_Fix
{
    using BepInEx;
    using HarmonyLib;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        public void Start()
        {
            Harmony.CreateAndPatchAll(typeof(Patches.BaseDeconstructable_Patches), PluginInfo.PLUGIN_GUID);
        }
    }
}