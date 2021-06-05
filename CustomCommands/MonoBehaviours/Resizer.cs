namespace CustomCommands.MonoBehaviours
{
    using UnityEngine;

    internal sealed class ResizerMono: MonoBehaviour
    {
        public void Start()
        {
            transform.localScale = Size;
        }

        public void Setsize(float scalex, float scaley, float scalez)
        {
            Size = new Vector3(scalex, scaley, scalez);
            transform.localScale = Size;
        }

        public Vector3 Size;
    }
}
