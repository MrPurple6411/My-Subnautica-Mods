namespace CustomCommands.MonoBehaviours
{
    using UnityEngine;

    public class Placeholder
    {
        public static void Awake()
        {
            if(DummyObject != null)
                Object.DestroyImmediate(DummyObject);
            DummyObject = new GameObject("DummyObject");
            DummyObject.AddComponent<SceneCleanerPreserve>();
            Object.DontDestroyOnLoad(DummyObject);
            DummyObject.AddComponent<Commands>();
        }

        private static GameObject DummyObject;
    }
}
