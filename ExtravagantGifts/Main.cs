#if BZ
namespace ExtravagantGifts
{
    using HarmonyLib;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
#region[Declarations]

        public const string
            MODNAME = "ExtravagantGifts",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

#endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Patches.Patches), $"MrPurple6411_ExtravagantGifts");
        }
    }
}
#endif