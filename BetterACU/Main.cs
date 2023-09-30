namespace BetterACU;

using Configuration;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;
using BepInEx;
using Nautilus.Utility;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    internal static SMLConfig SMLConfig { get; private set; }

    private void Awake()
    {
        SMLConfig = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        SaveUtils.RegisterOnSaveEvent(SMLConfig.Save);
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
    }
}