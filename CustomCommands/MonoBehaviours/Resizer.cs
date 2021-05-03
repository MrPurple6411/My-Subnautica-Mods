namespace CustomCommands.MonoBehaviours
{
    using UnityEngine;

    // Token: 0x02000003 RID: 3
    internal class Mono: MonoBehaviour
    {
        // Token: 0x0600000D RID: 13 RVA: 0x00002B1D File Offset: 0x00000D1D
        public virtual void Start()
        {
            base.transform.localScale = Size;
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002B32 File Offset: 0x00000D32
        public void Setsize(float scalex, float scaley, float scalez)
        {
            Size = new Vector3(scalex, scaley, scalez);
            base.transform.localScale = Size;
        }

        // Token: 0x04000008 RID: 8
        public Vector3 Size;
    }
}
