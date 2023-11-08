namespace CustomCommands;

using MonoBehaviours;
using BepInEx;
using UnityEngine;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    private static GameObject DummyObject;

    public void Awake()
    {
        Initialize();
    }

    internal static void Initialize()
    {
        if(DummyObject != null)
            Object.DestroyImmediate(DummyObject);
        DummyObject = new GameObject("DummyObject");
        DummyObject.AddComponent<SceneCleanerPreserve>();
        Object.DontDestroyOnLoad(DummyObject);
        DummyObject.AddComponent<Commands>();
    }
}