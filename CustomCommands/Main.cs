namespace CustomCommands
{
    using MonoBehaviours;
    using BepInEx;
    using UnityEngine;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "CustomCommands",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        private static GameObject DummyObject;
        #endregion

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
}