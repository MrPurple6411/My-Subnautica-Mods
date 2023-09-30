namespace Increased_Resource_Spawns;

using HarmonyLib;
using Configuration;
using Nautilus.Handlers;
using System.Linq;
using System.Reflection;
using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    internal static SMLConfig SMLConfig { get; } = OptionsPanelHandler.RegisterModOptions<SMLConfig>();

    private void Awake()
    {
        SMLConfig.Blacklist = SMLConfig.Blacklist.Distinct().ToList();
        SMLConfig.WhiteList = SMLConfig.WhiteList.Distinct().ToList();
        SMLConfig.Save();
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
    }
}