namespace SeamothThermal;

using HarmonyLib;
using System.Reflection;using BepInEx;
using SeamothThermal.Modules;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
public class Main: BaseUnityPlugin
{
    internal static TechType thermalModule;

    private void Awake()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
        thermalModule = new SeamothThermalModule().Info.TechType;
        Logger.LogInfo("Succesfully patched!");
    }
}