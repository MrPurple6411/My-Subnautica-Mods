namespace CustomCommands.MonoBehaviours
{
    using UnityEngine;

    // Token: 0x02000004 RID: 4
    public class Placeholder: MonoBehaviour
    {
        // Token: 0x06000010 RID: 16 RVA: 0x00002B60 File Offset: 0x00000D60
        public static void Awake()
        {
            try
            {
                GameObject.Destroy(Placeholder.DummyObject);
                Placeholder.DummyObject = new GameObject("DummyObject");
                GameObject.DontDestroyOnLoad(Placeholder.DummyObject);
                Placeholder.DummyObject.AddComponent<Commands>();
            }
            catch
            {
                Placeholder.DummyObject = new GameObject("DummyObject");
                GameObject.DontDestroyOnLoad(Placeholder.DummyObject);
                Placeholder.DummyObject.AddComponent<Commands>();
            }
        }

        // Token: 0x04000009 RID: 9
        public static GameObject DummyObject;
    }
}
