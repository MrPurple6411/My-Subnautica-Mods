namespace BuilderModule;

using Module;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
#if BELOWZERO
using Nautilus.Handlers;
#endif

using BepInEx;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    internal readonly Assembly assembly = Assembly.GetExecutingAssembly();
    internal static ManualLogSource logSource;
    internal static readonly List<TechType> BuilderModules = new();

    internal void Awake()
    {
        logSource = Logger;
        var builderModule = new BuilderModulePrefab();
        BuilderModules.Add(builderModule.Info.TechType);

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
    }
}