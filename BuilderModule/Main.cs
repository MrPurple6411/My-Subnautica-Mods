namespace BuilderModule;

using Module;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
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