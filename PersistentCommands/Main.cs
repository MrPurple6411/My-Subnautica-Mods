namespace PersistentCommands;

using HarmonyLib;
using Configuration;
using Nautilus.Handlers;using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();

    private void Awake()
    {
        Harmony.CreateAndPatchAll(typeof(Patches.Patches), MyPluginInfo.PLUGIN_GUID);
    }
}