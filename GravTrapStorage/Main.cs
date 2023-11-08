namespace GravTrapStorage;

using System.Collections;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using Configuration;
using Nautilus.Handlers;

using BepInEx;
using BepInEx.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
public class Main: BaseUnityPlugin
{
    public static SMLConfig SMLConfig { get; private set; }
    internal static ManualLogSource logSource;

    private void Awake()
    {
        logSource = Logger;
        SMLConfig = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
        
        var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
        harmony.Patch(
            AccessTools.Method(
                typeof(PlatformUtils), nameof(PlatformUtils.PlatformInitAsync)
            ),
            postfix: new HarmonyMethod(AccessTools.Method(typeof(Main), nameof(Main.Postfix)))
        );
        Logger.Log(LogLevel.Info, $" Loaded.");
    }

    public static IEnumerator Postfix(IEnumerator result)
    {
        yield return result;
        logSource.Log(LogLevel.Debug, $" Starting Coroutine.");
        yield return ModifyGravspherePrefab();
    }
    
    public static IEnumerator ModifyGravspherePrefab()
    {
        logSource.Log(LogLevel.Debug, $" Attempting to Attaching Storage");
        CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(TechType.Gravsphere, false);
        yield return request;

        var prefab = request.GetResult();
        logSource.Log(LogLevel.Debug, $" Ensuring COI");
        var coi = prefab.transform.GetChild(0)?.gameObject.EnsureComponent<ChildObjectIdentifier>();
        
        if (coi)
        {
            logSource.Log(LogLevel.Debug, $"Attaching Storage");
            coi.classId = "GravTrapStorage";
            var storageContainer = coi.gameObject.EnsureComponent<StorageContainer>();
            storageContainer.prefabRoot = prefab;
            storageContainer.storageRoot = coi;

            storageContainer.width = SMLConfig.Width;
            storageContainer.height = SMLConfig.Height;
            storageContainer.storageLabel = "Grav trap";
        }
        else
        {
            logSource.Log(LogLevel.Error, $"Failed to add COI. Unable to attach storage!");
        }
    }
}