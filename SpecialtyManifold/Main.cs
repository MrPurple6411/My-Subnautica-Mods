namespace SpecialtyManifold;

using HarmonyLib;using BepInEx;
using SpecialtyManifold.Patches;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
public class Main: BaseUnityPlugin
{
    private void Awake()
    {
        Harmony.CreateAndPatchAll(typeof(Player_Update_Patch), MyPluginInfo.PLUGIN_GUID);
    }
}