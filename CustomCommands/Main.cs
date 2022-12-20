namespace CustomCommands
{
    using MonoBehaviours;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {

        #region[Declarations]

        public const string
            MODNAME = "CustomCommands",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        public readonly string modFolder;

        #endregion

        private void Awake()
        {
            Placeholder.Awake();
        }
    }
}