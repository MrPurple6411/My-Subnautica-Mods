namespace PowerOrder;

using HarmonyLib;
using Configuration;
using Nautilus.Handlers;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    internal static SMLConfig SMLConfig = new();
    internal static ManualLogSource logSource;

    private void Awake()
    {
        logSource = Logger;
        OptionsPanelHandler.RegisterModOptions(new Options());
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
        logSource.LogInfo("Patching complete.");
    }
}