namespace SeamothCloneTest
{
    using Prefabs;
    using BepInEx;
    
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main: BaseUnityPlugin
    {
        public void Start()
        {
            new SeamothClone().Patch();
        }
    }
}